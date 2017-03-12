using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        private readonly Dictionary<ITableEntity, string> newEntities = new Dictionary<ITableEntity, string>(new TableEntityEqualityComparer());
        private readonly Dictionary<ITableEntity, string> dirtyEntities = new Dictionary<ITableEntity, string>(new TableEntityEqualityComparer());
        private readonly Dictionary<ITableEntity, string> removedEntities = new Dictionary<ITableEntity, string>(new TableEntityEqualityComparer());
        private readonly Dictionary<ITableEntity, string> cleanEntities = new Dictionary<ITableEntity, string>(new TableEntityEqualityComparer());
        private readonly Dictionary<string, CloudTable> tables = new Dictionary<string, CloudTable>();
        private readonly CloudTableClient client;
        private readonly ITableStorageOptions options;
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public IEnumerable<ITableEntity> Entities => cleanEntities.Keys
                                                        .Union(newEntities.Keys)
                                                        .Union(dirtyEntities.Keys)
                                                        .Union(removedEntities.Keys);

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
        /// Gets the entity from local, if exists, or loads it from table storage.
        /// </summary>
        public async Task<TEntity> GetAsync<TEntity>(string partitionKey, string rowKey, string table, 
                                        CancellationToken cancellationToken, 
                                        Func<TableRequestOptions> requestOptions = null, 
                                        Func<OperationContext> operationContext = null) where TEntity : ITableEntity
        {
            var entity = GetLocal<TEntity>(partitionKey, rowKey);
            if (entity != null) return entity;

            var retrieveOperation = TableOperation.Retrieve(partitionKey, rowKey);
            var cloudTable = await GetOrCreateTableAsync(table, cancellationToken, requestOptions, operationContext);
            entity = (TEntity) (await cloudTable.ExecuteAsync(retrieveOperation, requestOptions?.Invoke(), operationContext?.Invoke(), cancellationToken)).Result;

            RegisterClean(entity, table);

            return entity;
        }

        /// <summary>
        /// Runs the segmented query on the cloud table to load all entities for the specified PartitionKey.
        /// </summary>
        public async Task<TableQuerySegment<TEntity>> GetAllAsync<TEntity>(string partitionKey, string table, TableContinuationToken continuationToken) where TEntity : ITableEntity, new()
        {
            var cloudTable = await GetOrCreateTableAsync(table);
            var query = new TableQuery<TEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));
            var result = await cloudTable.ExecuteQuerySegmentedAsync(query, continuationToken);
            return result;
        }

        /// <summary>
        /// Runs the query on the cloud table.
        /// </summary>
        public async Task<TableQuerySegment<TEntity>> FindAsync<TEntity>(TableQuery<TEntity> query, string table, TableContinuationToken continuationToken) where TEntity : ITableEntity, new()
        {
            var cloudTable = await GetOrCreateTableAsync(table);
            return await cloudTable.ExecuteQuerySegmentedAsync(query, continuationToken);
        }

        /// <summary>
        /// Triggers execution of operations stored in the local queue asynchronously.
        /// </summary>
        /// <returns>Number of records affected.</returns>
        public async Task<int> SaveChangesAsync(Func<TableRequestOptions> requestOptions = null, Func<OperationContext> operationContext = null)
        {
            return await SaveChangesAsync(CancellationToken.None, requestOptions, operationContext);
        }

        /// <summary>
        /// Triggers execution of operations stored in the local queue asynchronously.
        /// </summary>
        /// <returns>Number of records affected.</returns>
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken, Func<TableRequestOptions> requestOptions = null, Func<OperationContext> operationContext = null)
        {
            //await Test(cancellationToken);
            await semaphore.WaitAsync(cancellationToken);
            var affectedRecordsCount = 0;
            try
            {
                affectedRecordsCount += await InsertNewEntitiesAsync(cancellationToken, requestOptions, operationContext);
                affectedRecordsCount += await UpdateDirtyEntitiesAsync(cancellationToken, requestOptions, operationContext);
                affectedRecordsCount += await DeleteRemovedEntitiesAsync(cancellationToken, requestOptions, operationContext);
            }
            finally
            {
                semaphore.Release();
            }
            return affectedRecordsCount;
        }

        /// <summary>
        /// Gets the CloudTable instance. 
        /// </summary>
        /// <param name="tableName">Gets existing or creates a new instance of CloudTable</param>
        public async Task<CloudTable> GetOrCreateTableAsync(string tableName)
        {
            return await GetOrCreateTableAsync(tableName, CancellationToken.None, null, null);
        }

        /// <summary>
        /// Gets the CloudTable instance. 
        /// </summary>
        /// <param name="tableName">Gets existing or creates a new instance of CloudTable</param>
        /// <param name="cancellationToken"></param>
        /// <param name="requestOptions"></param>
        /// <param name="operationContext"></param>
        public async Task<CloudTable> GetOrCreateTableAsync(string tableName, CancellationToken cancellationToken, Func<TableRequestOptions> requestOptions, Func<OperationContext> operationContext)
        {
            CloudTable table;
            if (tables.TryGetValue(tableName, out table))
            {
                return table;
            }

            table = client.GetTableReference(tableName);
            await table.CreateIfNotExistsAsync(requestOptions?.Invoke(), operationContext?.Invoke(), cancellationToken);
            tables.Add(tableName, table);
            return table;
        }

        /// <summary>
        /// Triggers queries to insert new entities.
        /// </summary>
        /// <returns>Number of records processed.</returns>
        protected virtual async Task<int> InsertNewEntitiesAsync(CancellationToken cancellationToken, Func<TableRequestOptions> requestOptions = null, Func<OperationContext> operationContext = null)
        {
            // todo: improve logic to use batches; there are few conditions to be met though, so leaving it for future improvement
            var processedRecords = 0;
            var tableNames = newEntities.Select(x => x.Value).Distinct();
            foreach (var tableName in tableNames)
            {
                var table = await GetOrCreateTableAsync(tableName, cancellationToken, requestOptions, operationContext);
                foreach (var entity in newEntities.Keys)
                {
                    var operation = TableOperation.Insert(entity);
                    await table.ExecuteAsync(operation, requestOptions?.Invoke(), operationContext?.Invoke(), cancellationToken);
                    
                    processedRecords++;
                }
            }
            newEntities.Clear();
            return processedRecords;
        }

        /// <summary>
        /// Triggers queries to update entities.
        /// </summary>
        /// <returns>Number of records processed.</returns>
        protected virtual async Task<int> UpdateDirtyEntitiesAsync(CancellationToken cancellationToken, Func<TableRequestOptions> requestOptions = null, Func<OperationContext> operationContext = null)
        {
            // todo: improve logic to use batches; there are few conditions to be met though, so leaving it for future improvement
            var processedRecords = 0;
            var tableNames = dirtyEntities.Select(x => x.Value).Distinct();
            foreach (var tableName in tableNames)
            {
                var table = await GetOrCreateTableAsync(tableName, cancellationToken, requestOptions, operationContext);
                foreach (var entity in dirtyEntities.Keys)
                {
                    var operation = TableOperation.Replace(entity);
                    await table.ExecuteAsync(operation, requestOptions?.Invoke(), operationContext?.Invoke(), cancellationToken);
                    processedRecords++;
                }
            }
            dirtyEntities.Clear();
            return processedRecords;
        }

        /// <summary>
        /// Triggers queries to delete entities.
        /// </summary>
        /// <returns>Number of records processed.</returns>
        protected virtual async Task<int> DeleteRemovedEntitiesAsync(CancellationToken cancellationToken, Func<TableRequestOptions> requestOptions = null, Func<OperationContext> operationContext = null)
        {
            // todo: improve logic to use batches; there are few conditions to be met though, so leaving it for future improvement
            var processedRecords = 0;
            var tableNames = dirtyEntities.Select(x => x.Value).Distinct();
            foreach (var tableName in tableNames)
            {
                var table = await GetOrCreateTableAsync(tableName, cancellationToken, requestOptions, operationContext);
                foreach (var entity in removedEntities.Keys)
                {
                    var operation = TableOperation.Delete(entity);
                    await table.ExecuteAsync(operation, requestOptions?.Invoke(), operationContext?.Invoke(), cancellationToken);
                    processedRecords++;
                }
            }
            removedEntities.Clear();
            return processedRecords;
        }

        /// <summary>
        /// Registers an entity to collection of clean objects.
        /// </summary>
        public void RegisterClean(ITableEntity entity, string table)
        {
            if (cleanEntities.ContainsKey(entity))
                return;

            GuardObjectIsNotAlreadyIn(entity, newEntities);
            GuardObjectIsNotAlreadyIn(entity, dirtyEntities);
            GuardObjectIsNotAlreadyIn(entity, removedEntities);

            cleanEntities.Add(entity, table);
        }

        /// <summary>
        /// Registers an entity to collection of new objects.
        /// </summary>
        public void RegisterNew(ITableEntity entity, string table)
        {
            GuardObjectIsNotAlreadyIn(entity, cleanEntities);
            GuardObjectIsNotAlreadyIn(entity, dirtyEntities);
            GuardObjectIsNotAlreadyIn(entity, removedEntities);

            newEntities.Add(entity, table);
        }

        /// <summary>
        /// Registers an entity to collection of dirty objects.
        /// </summary>
        public void RegisterDirty(ITableEntity entity, string table)
        {
            GuardObjectIsNotAlreadyIn(entity, removedEntities);

            cleanEntities.Remove(entity);
            if (!dirtyEntities.ContainsKey(entity) && !newEntities.ContainsKey(entity))
                dirtyEntities.Add(entity, table);
        }

        /// <summary>
        /// Registers an entity to collection of removed objects.
        /// </summary>
        public void RegisterRemoved(ITableEntity entity, string table)
        {
            cleanEntities.Remove(entity);
            if (newEntities.Remove(entity))
                return;
            dirtyEntities.Remove(entity);
            if (!removedEntities.ContainsKey(entity))
                removedEntities.Add(entity, table);

        }

        /// <summary>
        /// Gets the entity stored in local identity map.
        /// </summary>
        private TEntity GetLocal<TEntity>(string partitionKey, string rowKey) where TEntity: ITableEntity
        {
            return (TEntity) Entities.SingleOrDefault(x => x.PartitionKey == partitionKey && x.RowKey == rowKey);
        }

        private static void GuardObjectIsNotAlreadyIn(ITableEntity entity, IDictionary<ITableEntity, string> dictionary)
        {
            if (dictionary.ContainsKey(entity))
                throw new InvalidOperationException("The entity is already in another dictionary.");
        }
    }
}
