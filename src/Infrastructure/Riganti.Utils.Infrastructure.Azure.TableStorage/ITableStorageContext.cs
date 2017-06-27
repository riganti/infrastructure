using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Riganti.Utils.Infrastructure.Azure.TableStorage.TableEntityMappers;

namespace Riganti.Utils.Infrastructure.Azure.TableStorage
{
    public interface ITableStorageContext
    {
        ITableEntityMapper TableEntityMapper { get; }
        IEnumerable<ITableEntity> Entities { get; }

        Task DeleteTableAsync(string tableName);
        Task DeleteTableAsync(string tableName, CancellationToken cancellationToken, TableRequestOptions requestOptions, OperationContext operationContext);
        Task<TableQuerySegment<TEntity>> FindAsync<TEntity>(TableQuery<TEntity> query, TableContinuationToken continuationToken) where TEntity : ITableEntity, new();
        Task<TableQuerySegment<TEntity>> GetAllAsync<TEntity>(string partitionKey, TableContinuationToken continuationToken) where TEntity : ITableEntity, new();
        Task<TEntity> GetAsync<TEntity>(string partitionKey, string rowKey)  where TEntity : ITableEntity, new();
        Task<TEntity> GetAsync<TEntity>(string partitionKey, string rowKey, CancellationToken cancellationToken, TableRequestOptions requestOptions = null, OperationContext operationContext = null) where TEntity : ITableEntity, new();
        Task<CloudTable> GetOrCreateTableAsync(string tableName);
        Task<CloudTable> GetOrCreateTableAsync(string tableName, CancellationToken cancellationToken, TableRequestOptions requestOptions, OperationContext operationContext);
        void RegisterClean(ITableEntity entity);
        void RegisterDirty(ITableEntity entity);
        void RegisterNew(ITableEntity entity);
        void RegisterRemoved(ITableEntity entity);
        Task<int> SaveChangesAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken, TableRequestOptions requestOptions = null, OperationContext operationContext = null);
    }
}