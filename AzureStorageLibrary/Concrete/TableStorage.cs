using AzureStorageLibrary.Abstract;
using AzureStorageLibrary.Constant;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorageLibrary.Concrete
{
    public class TableStorage<TEntity> : INoSqlStorage<TEntity> where TEntity : TableEntity, new()
    {
        private readonly CloudTableClient _cloudTableClient;
        private readonly CloudTable _cloudTable;

        public TableStorage()
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(AzureStorageConstant.AzureStorageConnectionString);
            _cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            _cloudTable = _cloudTableClient.GetTableReference(typeof(TEntity).Name);
            _cloudTable.CreateIfNotExists();
        }

        public async Task<TEntity> Add(TEntity entity)
        {
            var operation = TableOperation.InsertOrMerge(entity);
            var execute = await _cloudTable.ExecuteAsync(operation);
            return execute.Result as TEntity;
        }

        public IQueryable<TEntity> All()
        {
            return _cloudTable.CreateQuery<TEntity>().AsQueryable();
        }

        public async Task Delete(string rowKey, string partitionKey)
        {
            var entity = await Get(rowKey, partitionKey);
            var operation = TableOperation.Delete(entity);
            await _cloudTable.ExecuteAsync(operation);
        }

        public async Task<TEntity> Get(string rowKey, string partitonKey)
        {
            var operation = TableOperation.Retrieve<TEntity>(partitonKey, rowKey);
            var execute = await _cloudTable.ExecuteAsync(operation);
            return execute.Result as TEntity;
        }

        public IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> query)
        {
            return _cloudTable.CreateQuery<TEntity>().Where(query);
        }

        public async Task<TEntity> Update(TEntity entity)
        {
            var operation = TableOperation.Replace(entity);
            var execute = await _cloudTable.ExecuteAsync(operation);
            return execute.Result as TEntity;
        }
    }
}
