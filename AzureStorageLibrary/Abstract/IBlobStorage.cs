using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorageLibrary.Abstract
{
    public enum EContainerName
    {
        pictures,
        pdf,
        logs,
        watermarkpictures
    }

    public interface IBlobStorage
    {
        public string BlobUrl { get;}
        Task UploadAsync(Stream fileStream, string fileName, EContainerName eContanierName);
        Task<Stream> DownloadAsync(string fileName, EContainerName eContanierName);
        Task DeleteAsync(string fileName, EContainerName eContanierName);
        List<string> GetNames(EContainerName eContanierName);
    }
}
