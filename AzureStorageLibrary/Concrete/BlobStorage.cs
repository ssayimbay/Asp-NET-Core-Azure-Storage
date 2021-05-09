using Azure.Storage.Blobs;
using AzureStorageLibrary.Abstract;
using AzureStorageLibrary.Constant;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorageLibrary.Concrete
{
    public class BlobStorage : IBlobStorage
    {
        public string BlobUrl { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        private readonly BlobServiceClient _blobServiceClient;

        public BlobStorage()
        {
            _blobServiceClient = new BlobServiceClient(ConnectionStrings.AzureStorageConnectionString);
        }

        public async Task DeleteAsync(string fileName, EContanierName eContanierName)
        {
            var contanierClient = _blobServiceClient.GetBlobContainerClient(eContanierName.ToString());
            var blobClient = contanierClient.GetBlobClient(fileName);
            await blobClient.DeleteAsync();
        }

        public async Task<Stream> DownloadAsync(string fileName, EContanierName eContanierName)
        {
            var contanierClient = _blobServiceClient.GetBlobContainerClient(eContanierName.ToString());
            var blobClient = contanierClient.GetBlobClient(fileName);
            var info = await blobClient.DownloadAsync();
            return info.Value.Content;
        }

        public List<string> GetNames(EContanierName eContanierName)
        {
            List<string> blobNames = new List<string>();
            var contanierClient = _blobServiceClient.GetBlobContainerClient(eContanierName.ToString());
            var blobs = contanierClient.GetBlobs();
            blobs.ToList().ForEach(blob =>
            {
                blobNames.Add(blob.Name);
            });
            return blobNames;
        }

        public async Task UploadAsync(Stream fileStream, string fileName, EContanierName eContanierName)
        {
            var contanierClient = _blobServiceClient.GetBlobContainerClient(eContanierName.ToString());
            await contanierClient.CreateIfNotExistsAsync();
            await contanierClient.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
            var blobClient = contanierClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileStream, true);
        }
    }
}
