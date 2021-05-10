using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorageLibrary.StorageModel
{
   public class PictureWaterMarkQueque
    {
        public string UserId { get; set; }
        public string UserPartitionKey { get; set; }
        public List<string> Pictures { get; set; }
        public string ConnectionId { get; set; }
        public string WatermarkText { get; set; }

    }
}
