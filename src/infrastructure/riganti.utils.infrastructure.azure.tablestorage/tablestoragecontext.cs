using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Riganti.Utils.Infrastructure.Azure.TableStorage.TableEntityMappers;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.Azure.TableStorage
{
    /// <summary>
    /// Interface for common CRUD operations on Azure Table Storage tables. It also provides in-memory caching of loaded objects.
    /// </summary>
    public class TableStorageContext : StorageContext, ITableStorageContext
    {
        private readonly ITableEntityMapper tableEntityMapper;
        private readonly HashSet<ITableEntity> newEntities;
        private readonly HashSet<ITableEntity> dirtyEntities;
        private readonly HashSet<ITableEntity> removedEntities;
        private readonly HashSet<ITableEntity> cleanEntities;
        private readonly Dictionary<string, CloudTable> tables = new Dictionary<string, CloudTable>();
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        private CloudTableClient client;

        public ITableEntityMapper TableEntityMapper => tableEntityMapper;

        /// <summary>
        /// Gets all the entities registered within this context.
        /// </summary>
        public IEnumerable<ITableEntity> Entities => cleanEntities
                                                        .Union(newEntities)
                                                        .Union(dirtyEntities)
                                                        .Union(removedEntities);

        /// <summary>
        /// Creates a new <see cref="CloudTableClient"/> instance.
        /// </summary>
        protected virtual CloudTableClient Client => client ?? (client = StorageAccount.CreateCloudTableClient());


        /// <summary>
        /// Initializes a new <see cref="TableStorageContext"/> instance.
        /// </summary>
        public TableStorageContext(IStorageOptions options, ITableEntityMapper mapperRegistry = null) : base(options)
        {
            var comparer = new TableEntityEqualityComparer<ITableEntity>();
            this.tableEntityMapper = mapperRegistry ?? new AggregateTableEntityMapper(new RegistryTableEntityMapper(), new AttributeTableEntityMapper(), new TypeNameTableEntityMapper());
            newEntities = new HashSet<ITableEntity>(comparer);
            dirtyEntities = new HashSet<ITableEntity>(comparer);
            removedEntities = new HashSet<ITableEntity>(comparer);
            cleanEntities = new HashSet<ITableEntity>(comparer);
        }

        /// <summary>
        /// Gets the entity from identity map, if exists, or loads it from the table storage.
        /// </summary>
        public async Task<TEntity> GetAsync<TEntity>(string partitionKey, string rowKey) where TEntity : ITableEntity, new()
        {
            return await GetAsync<TEntity>(partitionKey, rowKey, CancellationToken.None);
        }

        /// <summary>
        /// Gets the entity from identity map, if exists, or loads it from the table storage.
        /// </summary>
        public async Task<TEntity> GetAsync<TEntity>(string partitionKey, string rowKey,
                                        CancellationToken cancellationToken,
                                        TableRequestOptions requestOptions = null,
                                        OperationContext operationContext = null) where TEntity : ITableEntity, new()
        {
            var entity = GetLocal<TEntity>(partitionKey, rowKey);
            if (entity != null) return entity;

            var retrieveOperation = TableOperation.Retrieve<TEntity>(partitionKey, rowKey);
            var cloudTable = await GetOrCreateTableAsync(tableEntityMapper.GetTable<TEntity>(), cancellationToken, requestOptions, operationContext);
            var tableResult = await cloudTable.ExecuteAsync(retrieveOperation, requestOptions, operationContext, cancellationToken);
            if (tableResult == null)
                return default(TEntity);

            entity = (TEntity)tableResult.Result;

            RegisterClean(entity);

            return entity;
        }

        /// <summary>
        /// Runs the segmented query on the cloud table to load all entities for the specified PartitionKey.
        /// </summary>
        public async Task<TableQuerySegment<TEntity>> GetAllAsync<TEntity>(string partitionKey, TableContinuationToken continuationToken) where TEntity : ITableEntity, new()
        {
            var cloudTable = await GetOrCreateTableAsync(tableEntityMapper.GetTable<TEntity>());
            var query = new TableQuery<TEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));
            var result = await cloudTable.ExecuteQuerySegmentedAsync(query, continuationToken);
            return result;
        }

        /// <summary>
        /// Runs the query on the cloud table.
        /// </summary>
        public async Task<TableQuerySegment<TEntity>> FindAsync<TEntity>(TableQuery<TEntity> query, TableContinuationToken continuationToken) where TEntity : ITableEntity, new()
        {
            var cloudTable = await GetOrCreateTableAsync(tableEntityMapper.GetTable<TEntity>());
            return await cloudTable.ExecuteQuerySegmentedAsync(query, continuationToken);
        }

        /// <summary>
        /// Triggers execution of operations stored in the local queue asynchronously.
        /// </summary>
        /// <returns>Number of records affected.</returns>
        public async Task<int> SaveChangesAsync()
        {
            return await SaveChangesAsync(CancellationToken.None);
        }

        /// <summary>
        /// Triggers execution of operations stored in the local queue asynchronously.
        /// </summary>
        /// <returns>Number of records affected.</returns>
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken, TableRequestOptions requestOptions = null, OperationContext operationContext = null)
        {
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
        /// Gets existing or creates a new instance of CloudTable 
        /// </summary>
        public async Task<CloudTable> GetOrCreateTableAsync(string tableName, CancellationToken cancellationToken, TableRequestOptions requestOptions, OperationContext operationContext)
        {
            CloudTable table;
            if (tables.TryGetValue(tableName, out table))
            {
                return table;
            }

            table = Client.GetTableReference(tableName);
            await table.CreateIfNotExistsAsync(requestOptions, operationContext, cancellationToken);
            tables.Add(tableName, table);
            return table;
        }

        /// <summary>
        /// Deletes the table.
        /// </summary>
        public async Task DeleteTableAsync(string tableName)
        {
            await DeleteTableAsync(tableName, CancellationToken.None, null, null);
        }

        /// <summary>
        /// Deletes the table.
        /// </summary>
        public async Task DeleteTableAsync(string tableName, CancellationToken cancellationToken, TableRequestOptions requestOptions, OperationContext operationContext)
        {
            var table = Client.GetTableReference(tableName);
            var exists = await table.ExistsAsync(requestOptions, operationContext, cancellationToken);
            if (exists)
            {
                await table.DeleteAsync(requestOptions, operationContext, cancellationToken);
            }
        }

        /// <summary>
        /// Registers an entity to collection of clean objects.
        /// </summary>
        public void RegisterClean(ITableEntity entity)
        {
            if (cleanEntities.Contains(entity))
                return;

            GuardObjectIsNotAlreadyIn(entity, newEntities);
            GuardObjectIsNotAlreadyIn(entity, dirtyEntities);
            GuardObjectIsNotAlreadyIn(entity, removedEntities);

            cleanEntities.Add(entity);
        }

        /// <summary>
        /// Registers an entity to collection of new objects.
        /// </summary>
        public void RegisterNew(ITableEntity entity)
        {
            GuardObjectIsNotAlreadyIn(entity, cleanEntities);
            GuardObjectIsNotAlreadyIn(entity, dirtyEntities);
            GuardObjectIsNotAlreadyIn(entity, removedEntities);

            newEntities.Add(entity);
        }

        /// <summary>
        /// Registers an entity to collection of dirty objects.
        /// </summary>
        public void RegisterDirty(ITableEntity entity)
        {
            GuardObjectIsNotAlreadyIn(entity, removedEntities);

            cleanEntities.Remove(entity);
            if (!dirtyEntities.Contains(entity) && !newEntities.Contains(entity))
                dirtyEntities.Add(entity);
        }

        /// <summary>
        /// Registers an entity to collection of removed objects.
        /// </summary>
        public void RegisterRemoved(ITableEntity entity)
        {
            cleanEntities.Remove(entity);
            if (newEntities.Remove(entity))
                return;
            dirtyEntities.Remove(entity);
            if (!removedEntities.Contains(entity))
                removedEntities.Add(entity);

        }

        /// <summary>
        /// Triggers queries to insert new entities.
        /// </summary>
        /// <returns>Number of records processed.</returns>
        protected virtual async Task<int> InsertNewEntitiesAsync(CancellationToken cancellationToken, TableRequestOptions requestOptions = null, OperationContext operationContext = null)
        {
            var processedRecords = 0;
            foreach (var sameTableEntities in newEntities.GroupBy(x => x.GetType()))
            {
                var table = await GetOrCreateTableAsync(tableEntityMapper.GetTable(sameTableEntities.First()), cancellationToken, requestOptions, operationContext);
                var batch = new TableBatchOperation();
                foreach (var entity in sameTableEntities)
                {
                    batch.Insert(entity);
                }
                await ((BatchSafeCloudTable)table).ExecuteBatchSafeAsync(batch, requestOptions, operationContext, cancellationToken);
                processedRecords += batch.Count;
            }
            newEntities.Clear();
            return processedRecords;
        }

        /// <summary>
        /// Triggers queries to update entities.
        /// </summary>
        /// <returns>Number of records processed.</returns>
        protected virtual async Task<int> UpdateDirtyEntitiesAsync(CancellationToken cancellationToken, TableRequestOptions requestOptions = null, OperationContext operationContext = null)
        {
            var processedRecords = 0;
            foreach (var sameTableEntities in dirtyEntities.GroupBy(x => x.GetType()))
            {
                var table = await GetOrCreateTableAsync(tableEntityMapper.GetTable(sameTableEntities.First()), cancellationToken, requestOptions, operationContext);
                var batch = new TableBatchOperation();
                foreach (var entity in sameTableEntities)
                {
                    batch.Replace(entity);
                }
                await ((BatchSafeCloudTable)table).ExecuteBatchSafeAsync(batch, requestOptions, operationContext, cancellationToken);
                processedRecords += batch.Count;
            }
            dirtyEntities.Clear();
            return processedRecords;
        }

        /// <summary>
        /// Triggers queries to delete entities.
        /// </summary>
        /// <returns>Number of records processed.</returns>
        protected virtual async Task<int> DeleteRemovedEntitiesAsync(CancellationToken cancellationToken, TableRequestOptions requestOptions = null, OperationContext operationContext = null)
        {
            var processedRecords = 0;
            foreach (var sameTableEntities in removedEntities.GroupBy(x => x.GetType()))
            {
                var table = await GetOrCreateTableAsync(tableEntityMapper.GetTable(sameTableEntities.First()), cancellationToken, requestOptions, operationContext);
                var batch = new TableBatchOperation();
                foreach (var entity in sameTableEntities)
                {
                    batch.Delete(entity);
                }
                await ((BatchSafeCloudTable) table).ExecuteBatchSafeAsync(batch, requestOptions, operationContext, cancellationToken);
                processedRecords += batch.Count;
            }
            removedEntities.Clear();
            return processedRecords;
        }

        /// <summary>
        /// Gets the entity stored in local identity map.
        /// </summary>
        private TEntity GetLocal<TEntity>(string partitionKey, string rowKey) where TEntity : ITableEntity
        {
            return (TEntity)Entities.SingleOrDefault(x => x.PartitionKey == partitionKey && x.RowKey == rowKey);
        }

        private static void GuardObjectIsNotAlreadyIn(ITableEntity entity, HashSet<ITableEntity> entities)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (entities == null) throw new ArgumentNullException(nameof(entities));
            if (entities.Contains(entity)) throw new InvalidOperationException("The entity is already in another collection.");
        }

        
    }
}
