using AzureStorageLibrary.Abstract;
using AzureStorageLibrary.StorageModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaterMarkWeb.Models;

namespace WaterMarkWeb.Controllers
{
    public class PicturesController : Controller
    {
        private readonly INoSqlStorage<UserPicture> _noSqlStorage;
        private readonly IBlobStorage _blobStorage;
        private readonly IQueue _queue;

        public PicturesController(INoSqlStorage<UserPicture> noSqlStorage, IBlobStorage blobStorage,IQueue queue)
        {
            _noSqlStorage = noSqlStorage;
            _blobStorage = blobStorage;
            _queue = queue;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.UserId = UserFakeDBModel.UserId;
            ViewBag.UserPartitionKey = UserFakeDBModel.UserPartitonKey;

            List<BlobViewModel> blobViewModels = new List<BlobViewModel>();

            var userPicture = await _noSqlStorage.Get(UserFakeDBModel.UserId, UserFakeDBModel.UserPartitonKey);
            if (userPicture != null)
            {
                userPicture.Paths.ForEach(x =>
                {
                    blobViewModels.Add(new BlobViewModel
                    {
                        Name = x,
                        Url = $"{_blobStorage.BlobUrl}/{EContanierName.pictures}/{x}"
                    });
                });
            }

            return View(blobViewModels);
        }

        [HttpPost]
        public async Task<IActionResult> Index(IEnumerable<IFormFile> pictures)
        {
            List<string> pictureList = new List<string>();
            foreach (var picture in pictures)
            {
                var newPictureName = $"{Guid.NewGuid()}{Path.GetExtension(picture.FileName)}";

                await _blobStorage.UploadAsync(picture.OpenReadStream(), newPictureName, EContanierName.pictures);
                pictureList.Add(newPictureName);
            }

            var isUser = await _noSqlStorage.Get(UserFakeDBModel.UserId, UserFakeDBModel.UserPartitonKey);

            if (isUser == null)
            {
                isUser = new UserPicture
                {
                    RowKey = UserFakeDBModel.UserId,
                    PartitionKey = UserFakeDBModel.UserPartitonKey,
                    Paths = pictureList
                };
            }
            else
            {
                pictureList.AddRange(isUser.Paths);
                isUser.Paths = pictureList;
            }

            await _noSqlStorage.Add(isUser);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddWaterMark(PictureWaterMarkQueque pictureWaterMarkQueque)
        {
            if(pictureWaterMarkQueque.Pictures == null)
            {
                return BadRequest();
            }

            var jsonString = JsonConvert.SerializeObject(pictureWaterMarkQueque);
            var jsonStringBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonString));
            await _queue.SendMessageAsync(jsonStringBase64);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> DeletePaths(string[] pictures)
        {
            var isUser = await _noSqlStorage.Get(UserFakeDBModel.UserId, UserFakeDBModel.UserPartitonKey);
            var userPictures = isUser.Paths;
            isUser.RawPaths = string.Empty;
            foreach (var picture in pictures)
            {
                await _blobStorage.DeleteAsync(picture, EContanierName.pictures);
                userPictures.Remove(picture);
            }
            isUser.Paths = userPictures;

            await _noSqlStorage.Update(isUser);

            return RedirectToAction("Index");
        }
    }
}
