using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Riganti.Utils.Infrastructure.Azure.TableStorage
{
    /// <summary>
    /// Interface for common CRUD operations on Azure Table Storage tables. It also provides in-memory caching of loaded objects.
    /// </summary>
    public class TableStorageContext
    {
        private readonly ITableStorageOptions options;
        private readonly ConcurrentDictionary<Tuple<string, string>, TableEntity> objectCache = new ConcurrentDictionary<Tuple<string, string>, TableEntity>(new TableEntityEqualityComparer());
        private readonly ConcurrentDictionary<string, ConcurrentQueue<TableOperation>> operationsBuffer = new ConcurrentDictionary<string, ConcurrentQueue<TableOperation>>();
        private readonly ConcurrentDictionary<string, CloudTable> tableCache = new ConcurrentDictionary<string, CloudTable>();
        private readonly CloudTableClient client;

        // keeps the connection to table storage
        private CloudStorageAccount StorageAccount => new Lazy<CloudStorageAccount>(
           () => CloudStorageAccount.Parse(options.StorageConnectionString)).Value;

        /// <summary>
        /// Initializes a new TableStorageContext instance.
        /// </summary>
        /// <param name="options"></param>
        public TableStorageContext(ITableStorageOptions options)
        {
            this.options = options;
            client = StorageAccount.CreateCloudTableClient();
        }

        /// <summary>
        /// Creates the table if not exists.
        /// </summary>
        public async void CreateTableIfNotExistsAsync(string tableName)
        {
            await GetTableAsync(tableName, true);
        }

        /// <summary>
        /// Gets the entity stored in local cache.
        /// </summary>
        /// <param name="keys">PartitionKey, RowKey</param>
        /// <returns>Entity object</returns>
        public TEntity GetLocal<TEntity>(Tuple<string, string> keys) where TEntity : TableEntity, new()
        {
            return (TEntity)objectCache[keys];
        }

        /// <summary>
        /// Gets the entity from local, if exists, or loads it from table storage.
        /// </summary>
        public async Task<TEntity> GetAsync<TEntity>(string table, Tuple<string, string> keys, CancellationToken cancellationToken, Func<TableRequestOptions> requestOptions = null, Func<OperationContext> operationContext = null) where TEntity : TableEntity, new()
        {
            var entity = GetLocal<TEntity>(keys);
            if (entity != null) return entity;

            var retrieveOperation = TableOperation.Retrieve(keys.Item1, keys.Item2);
            var cloudTable = await GetTableAsync(table);
            entity = (TEntity) (await cloudTable.ExecuteAsync(retrieveOperation, requestOptions?.Invoke(), operationContext?.Invoke(), cancellationToken)).Result;

            AddEntityToLocalCache(entity);

            return entity;
        }

        /// <summary>
        /// Inserts a new entity to the specified table.
        /// </summary>
        public void Insert<TEntity>(string table, TEntity entity) where TEntity : TableEntity, new()
        {
            var insertOperation = TableOperation.Insert(entity);
            var queue = operationsBuffer.GetOrAdd(table, new ConcurrentQueue<TableOperation>());
            queue.Enqueue(insertOperation);

            AddEntityToLocalCache(entity);
        }

        /// <summary>
        /// Inserts new entities to the specified table.
        /// </summary>
        public void Insert<TEntity>(string table, IEnumerable<TEntity> entities) where TEntity : TableEntity, new()
        {
            foreach (var entity in entities)
                Insert(table, entity);
        }

        /// <summary>
        /// Updates the entity in the specified table.
        /// </summary>
        public void Update<TEntity>(string table, TEntity entity) where TEntity : TableEntity, new()
        {
            var updateOperation = TableOperation.Replace(entity);
            var queue = operationsBuffer.GetOrAdd(table, new ConcurrentQueue<TableOperation>());
            queue.Enqueue(updateOperation);

            AddEntityToLocalCache(entity);
        }

        /// <summary>
        /// Updates the entities in the specified table.
        /// </summary>
        public void Update<TEntity>(string table, IEnumerable<TEntity> entities) where TEntity : TableEntity, new()
        {
            foreach (var entity in entities)
                Update(table, entity);
        }

        /// <summary>
        /// Deletes the entity from the specified table.
        /// </summary>
        public void Delete<TEntity>(string table, TEntity entity) where TEntity : TableEntity, new()
        {
            var deleteOperation = TableOperation.Delete(entity);
            var queue = operationsBuffer.GetOrAdd(table, new ConcurrentQueue<TableOperation>());
            queue.Enqueue(deleteOperation);

            AddEntityToLocalCache(entity);
        }

        /// <summary>
        /// Deletes the entity in the table.
        /// </summary>
        /// <param name="table">The table name</param>
        /// <param name="keys">PartitionKey, RowKey</param>
        public void Delete<TEntity>(string table, Tuple<string, string> keys) where TEntity : TableEntity, new()
        {
            var entity = GetLocal<TEntity>(keys) ?? GetAsync<TEntity>(table, keys, CancellationToken.None).Result;
            Delete(table, entity);
        }

        /// <summary>
        /// Deletes the entities from the specified table.
        /// </summary>
        public void Delete<TEntity>(string table, IEnumerable<TEntity> entities) where TEntity : TableEntity, new()
        {
            foreach (var entity in entities)
                Delete(table, entity);
        }

        /// <summary>
        /// Runs the query on the cloud table.
        /// </summary>
        public async Task<TableQuerySegment<TEntity>> FindAsync<TEntity>(string table, TableQuery<TEntity> query, TableContinuationToken continuationToken) where TEntity : TableEntity, new()
        {
            var cloudTable = await GetTableAsync(table);
            return await cloudTable.ExecuteQuerySegmentedAsync(query, continuationToken);
        }

        /// <summary>
        /// Runs the segmented query on the cloud table to load all entities.
        /// Filter usually contains PartitionKey.
        /// </summary>
        public async Task<TableQuerySegment<TEntity>> GetAllAsync<TEntity>(string table, string filter, TableContinuationToken continuationToken) where TEntity : TableEntity, new()
        {
            var cloudTable = await GetTableAsync(table);
            var tableQuery = new TableQuery<TEntity>();
            if (filter != null)
            {
                tableQuery = tableQuery.Where(filter);
            }
            return await cloudTable.ExecuteQuerySegmentedAsync(tableQuery, continuationToken);
        }

        /// <summary>
        /// Triggers execution of operations stored in the local buffer asynchronously.
        /// </summary>
        public async Task SaveChangesAsync(CancellationToken cancellationToken, Func<TableRequestOptions> requestOptions = null, Func<OperationContext> operationContext = null)
        {
            foreach (var table in operationsBuffer.Keys)
            {
                var cloudTable = await GetTableAsync(table);
                TableOperation operation;
                while (operationsBuffer[table].TryDequeue(out operation))
                {
                    await cloudTable.ExecuteAsync(operation, requestOptions?.Invoke(), operationContext?.Invoke(), cancellationToken);
                }
            }
            ClearLocalCaches();
        }

        /// <summary>
        /// Gets the CloudTable instance. 
        /// </summary>
        /// <param name="tableName">Gets existing or creates a new instance of CloudTable</param>
        /// <param name="createIfNotExists">Creates a new table if not exists</param>
        /// <returns></returns>
        internal async Task<CloudTable> GetTableAsync(string tableName, bool createIfNotExists = false)
        {
            var table = client.GetTableReference(tableName);
            if (createIfNotExists)
            {
                await table.CreateIfNotExistsAsync();
            }

            return tableCache.GetOrAdd(tableName, table);
        }

        /// <summary>
        /// Adds an entity to local cache.
        /// </summary>
        private void AddEntityToLocalCache(TableEntity entity)
        {
            if (entity == null)
                return;

            var key = new Tuple<string, string>(entity.PartitionKey, entity.RowKey);
            if (objectCache.ContainsKey(key))
                objectCache[key] = entity;
            else
                objectCache.TryAdd(key, entity);
        }

        /// <summary>
        /// Clears the local cache.
        /// </summary>
        private void ClearLocalCaches()
        {
            objectCache.Clear();
            tableCache.Clear();
            operationsBuffer.Clear();
        }
    }
}
