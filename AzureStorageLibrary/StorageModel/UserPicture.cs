using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorageLibrary.StorageModel
{
    public class UserPicture : TableEntity
    {
        public string RawPaths { get; set; }

        [IgnoreProperty]
        public List<string> Paths
        {
            get => RawPaths == null ? null : JsonConvert.DeserializeObject<List<string>>(RawPaths);
            set => RawPaths = JsonConvert.SerializeObject(value);
        }
        public string WatermarkRawPaths { get; set; }

        [IgnoreProperty]
        public List<string> WaterMarkPaths
        {
            get => WaterMarkPaths == null ? null : JsonConvert.DeserializeObject<List<string>>(WatermarkRawPaths);
            set => WatermarkRawPaths = JsonConvert.SerializeObject(value);
        }
    }
}
