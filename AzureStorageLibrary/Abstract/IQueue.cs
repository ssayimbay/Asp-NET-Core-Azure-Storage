using Azure.Storage.Queues.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorageLibrary.Abstract
{
   public interface IQueue
    {
        public Task SendMessageAsync(string message);
        public Task<QueueMessage> RetrieveNextMessageAsync();
        public Task DeleteMessage(string messageId, string popReceipt);

    }
}
