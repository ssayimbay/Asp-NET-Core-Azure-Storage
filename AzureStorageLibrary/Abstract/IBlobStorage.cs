using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorageLibrary.Abstract
{
    public enum EContanierName
    {
        pictures,
        pdf,
        logs,
        watermarkpictures
    }

    public interface IBlobStorage
    {
        public string BlobUrl { get;}
        Task UploadAsync(Stream fileStream, string fileName, EContanierName eContanierName);
        Task<Stream> DownloadAsync(string fileName, EContanierName eContanierName);
        Task DeleteAsync(string fileName, EContanierName eContanierName);
        List<string> GetNames(EContanierName eContanierName);
    }
}
