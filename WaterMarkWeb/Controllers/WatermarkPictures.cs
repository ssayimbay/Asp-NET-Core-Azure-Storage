using AzureStorageLibrary.Abstract;
using AzureStorageLibrary.StorageModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaterMarkWeb.Models;

namespace WaterMarkWeb.Controllers
{
    public class WatermarkPictures : Controller
    {
        private readonly INoSqlStorage<UserPicture> _noSqlStorage;
        private readonly IBlobStorage _blobStorage;
        private readonly IQueue _queue;

        public WatermarkPictures(INoSqlStorage<UserPicture> noSqlStorage, IBlobStorage blobStorage, IQueue queue)
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
                if(userPicture.WatermarkPaths != null)
                userPicture.WatermarkPaths.ForEach(x =>
                {
                    blobViewModels.Add(new BlobViewModel
                    {
                        Name = x,
                        Url = $"{_blobStorage.BlobUrl}/{EContainerName.watermarkpictures}/{x}"
                    });
                });
            }
            return View(blobViewModels);

        }

        [HttpPost]
        public async Task<IActionResult> DeletePaths(string[] pictures)
        {
            var isUser = await _noSqlStorage.Get(UserFakeDBModel.UserId, UserFakeDBModel.UserPartitonKey);
            var userPicturesList = isUser.WatermarkPaths;
            isUser.WatermarkRawPaths = string.Empty;
            foreach (var picture in pictures)
            {
                await _blobStorage.DeleteAsync(picture, EContainerName.watermarkpictures);
                userPicturesList.Remove(picture);
            }
            isUser.WatermarkPaths = userPicturesList;

            await _noSqlStorage.Update(isUser);

            return RedirectToAction("Index");
        }
    }
}
