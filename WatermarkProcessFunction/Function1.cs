using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using AzureStorageLibrary.Abstract;
using AzureStorageLibrary.Concrete;
using AzureStorageLibrary.Constant;
using AzureStorageLibrary.StorageModel;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace WatermarkProcessFunction
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public async static void Run([QueueTrigger("watermarkqueue", Connection = "")]PictureWaterMarkQueque myQueueItem, ILogger log)
        {
            try
            {
              //  AzureStorageConstant.AzureStorageConnectionString = "AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";
                IBlobStorage blobStorage = new BlobStorage();
                INoSqlStorage<UserPicture> noSqlStorage = new TableStorage<UserPicture>();
    

                foreach (var item in myQueueItem.Pictures)
                {
                    using var stream = await blobStorage.DownloadAsync(item, EContainerName.pictures);

                    using var memoryStream = AddWaterMark(myQueueItem.WatermarkText, stream);

                    await blobStorage.UploadAsync(memoryStream, item, EContainerName.watermarkpictures);

                    log.LogInformation($"{item} Pictures added for watermark.");
                }

                var userpicture = await noSqlStorage.Get(myQueueItem.UserId, myQueueItem.UserPartitionKey);

                if (userpicture.WatermarkRawPaths != null)
                {
                    myQueueItem.Pictures.AddRange(userpicture.WatermarkPaths);

                }

                userpicture.WatermarkPaths = myQueueItem.Pictures;

                await noSqlStorage.Add(userpicture);

                HttpClient httpClient = new HttpClient();

                var response = await httpClient.GetAsync("https://localhost:44392/api/Notifications/CompleteWatermarkProcess/" + myQueueItem.ConnectionId);

                log.LogInformation($"Client({myQueueItem.ConnectionId}) my process complate.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

            }
            
        }

        public static MemoryStream AddWaterMark(string watermarkText, Stream PictureStream)
        {
         
            MemoryStream ms = new MemoryStream();

            using (Image image = Bitmap.FromStream(PictureStream))
            {
                using (Bitmap tempBitmap = new Bitmap(image.Width, image.Height))
                {
                    using (Graphics gph = Graphics.FromImage(tempBitmap))
                    {
                        gph.DrawImage(image, 0, 0);

                        var font = new Font(FontFamily.GenericSansSerif, 25, FontStyle.Bold);

                        var color = Color.FromArgb(255, 0, 0);

                        var brush = new SolidBrush(color);

                        var point = new Point(20, image.Height - 50);

                        gph.DrawString(watermarkText, font, brush, point);

                        tempBitmap.Save(ms, ImageFormat.Png);
                    }
                }
            }

            ms.Position = 0;

            return ms;
        }
    }
}
