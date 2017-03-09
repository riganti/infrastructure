using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.Azure.TableStorage
{
    /// <summary>
    /// An implementation of repository in Azure Table Storage.
    /// </summary>
    /// <typeparam name="TEntity">A custom entity inherrited from TableEntity.</typeparam>
    public class TableStorageRepository<TEntity> : ITableStorageRepository<TEntity> where TEntity : TableEntity, new() 
    {
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly IUnitOfWorkProvider provider;

        protected TableStorageContext Context => TableStorageUnitOfWork.TryGetTableStorageContext(provider);
        
        public TableStorageRepository(IUnitOfWorkProvider provider, IDateTimeProvider dateTimeProvider)
        {
            this.provider = provider;
            this.dateTimeProvider = dateTimeProvider;
        }

        public void Delete(string table, TEntity entity)
        {
            Context.Delete(table, entity);
        }

        public void Delete(string table, IEnumerable<TEntity> entities)
        {
            Context.Delete(table, entities);
        }

        public void Delete(string table, Tuple<string, string> keys)
        {
            Context.Delete<TEntity>(table, keys);
        }

        public async Task<TEntity> GetByKeyAsync(string table, Tuple<string, string> keys, CancellationToken cancellationToken)
        {
            return await Context.GetAsync<TEntity>(table, keys, cancellationToken);
        }

        public TEntity InitializeNew(Tuple<string, string> keys)
        {
            return new TEntity
            {
                PartitionKey = keys.Item1,
                RowKey = keys.Item2,
                Timestamp = dateTimeProvider.Now
            };
        }

        public TableQuery<TEntity> InitializeNewQuery()
        {
            return new TableQuery<TEntity>();
        }

        public void Insert(string table, TEntity entity)
        {
            Context.CreateTableIfNotExistsAsync(table);
            Context.Insert(table, entity);
        }

        public void Insert(string table, IEnumerable<TEntity> entities)
        {
            Context.CreateTableIfNotExistsAsync(table);
            Context.Insert(table, entities);
        }

        public void Update(string table, TEntity entity)
        {
            Context.Update(table, entity);
        }

        public void Update(string table, IEnumerable<TEntity> entities)
        {
            Context.Update(table, entities);
        }

        public IEnumerable<TEntity> GetAll(string table, string partitionKey)
        {
            return GetAllAsync(table, partitionKey).Result;
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(string table, string partitionKey)
        {
            TableContinuationToken continuationToken = null;
            TableQuerySegment<TEntity> result;
            var filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey);
            do
            {
                result = await Context.GetAllAsync<TEntity>(table, filter, continuationToken);
                continuationToken = result.ContinuationToken;
            } while (continuationToken != null);

            return result.Results;
        }

        public async Task<IEnumerable<TEntity>> FindAsync(string table, TableQuery<TEntity> query)
        {
            TableContinuationToken continuationToken = null;
            TableQuerySegment<TEntity> result;
            do
            {
                result = await Context.FindAsync(table, query, continuationToken);
                continuationToken = result.ContinuationToken;
            } while (continuationToken != null);

            return result.Results;
        }
    }
}
