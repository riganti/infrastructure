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
    public class TableStorageRepository<TEntity> : ITableStorageRepository<TEntity> where TEntity : class, ITableEntity, new()
    {
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly IUnitOfWorkProvider provider;

        public ITableStorageContext Context => TableStorageUnitOfWork.TryGetTableStorageContext(provider);

        public TableStorageRepository(IUnitOfWorkProvider provider, IDateTimeProvider dateTimeProvider)
        {
            this.provider = provider;
            this.dateTimeProvider = dateTimeProvider;
        }

        public void Delete(TEntity entity)
        {
            Context.RegisterRemoved(entity);
        }

        public void Delete(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities) 
                Context.RegisterRemoved(entity);
        }

        public async void DeleteAsync(string partitionKey, string rowKey, CancellationToken cancellationToken)
        {
            var entity = await Context.GetAsync<TEntity>(partitionKey, rowKey, cancellationToken).ConfigureAwait(false);
            Delete(entity);
        }

        public Task<TEntity> GetByKeyAsync(string partitionKey, string rowKey)
        {
            return GetByKeyAsync(partitionKey, rowKey, CancellationToken.None);
        }

        public Task<TEntity> GetByKeyAsync(string partitionKey, string rowKey, CancellationToken cancellationToken)
        {
                        
            return Context.GetAsync<TEntity>(partitionKey, rowKey, cancellationToken);
        }

        public TEntity InitializeNew(string partitionKey, string rowKey)
        {
            return new TEntity
            {
                PartitionKey = partitionKey,
                RowKey = rowKey,
                Timestamp = dateTimeProvider.Now
            };
        }

        public TableQuery<TEntity> InitializeNewQuery()
        {
            return new TableQuery<TEntity>();
        } 

        public void Insert(TEntity entity)
        {
            Context.RegisterNew(entity);
        }

        public void Insert(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
                Insert(entity);
        }

        public void Update(TEntity entity)
        {
            Context.RegisterDirty(entity);
        }

        public void Update(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
                Update(entity);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(string partitionKey)
        {
            TableContinuationToken continuationToken = null;
            TableQuerySegment<TEntity> result;
            do
            {
                result = await Context.GetAllAsync<TEntity>(partitionKey, continuationToken).ConfigureAwait(false);
                continuationToken = result.ContinuationToken;
            } while (continuationToken != null);

            return result.Results;
        }

        public async Task<IEnumerable<TEntity>> FindAllAsync(TableQuery<TEntity> query)
        {
            TableContinuationToken continuationToken = null;
            TableQuerySegment<TEntity> result;
            do
            {
                result = await Context.FindAsync(query, continuationToken).ConfigureAwait(false);
                continuationToken = result.ContinuationToken;
            } while (continuationToken != null);

            return result.Results;
        }

        public Task<TableQuerySegment<TEntity>> FindAsync(TableQuery<TEntity> query, TableContinuationToken continuationToken)
        {
            return Context.FindAsync(query, continuationToken);
        }
    }
}
