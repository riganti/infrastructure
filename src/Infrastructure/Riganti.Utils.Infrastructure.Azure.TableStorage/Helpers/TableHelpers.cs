using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.Azure.TableStorage
{
    public static class TableHelpers
    {
        public static async Task<IEnumerable<T>> ExecuteQueryAsync<T>(this CloudTable table, TableQuery<T> query, TableContinuationToken token = null) where T : ITableEntity, new()
        {
            var entities = new List<T>();
            do
            {
                var queryResult = await table.ExecuteQuerySegmentedAsync(query, token);
                entities.AddRange(queryResult.Results);
                token = queryResult.ContinuationToken;
            } while (token != null);
            return entities;
        }


        /// <summary>
        /// Execute a batch of operations on a table. It handles API limits by executing only chunks of the batch which meet limit criteria.
        /// </summary>
        /// <param name="batch">The <see cref="TableBatchOperation"/> object representing the operations to execute on the table.</param>
        /// <param name="requestOptions">A <see cref="TableRequestOptions"/> object that specifies additional options for the request.</param>
        /// <param name="operationContext">An <see cref="OperationContext"/> object that represents the context for the current operation.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <param name="numberOfBatchesProcesedInParallel">Number of partial batch operations that will be executed simultaneously.</param>
        /// <returns>List of type <see cref="TableResult"/>.</returns>
        public static async Task<IList<TableResult>> ExecuteBatchSafeAsync(this CloudTable table, IEnumerable<TableOperation> batch, TableRequestOptions requestOptions, OperationContext operationContext,
            CancellationToken cancellationToken, int numberOfBatchesProcesedInParallel = 3)
        {
            var result = new List<TableResult>();

            foreach (var chunk in GetLimitConformingBatches(batch).ToList().ChunkBy(numberOfBatchesProcesedInParallel))
            {
                foreach (var batchResult in await Task.WhenAll(chunk.Select(b => table.ExecuteBatchAsync(b, requestOptions, operationContext, cancellationToken))))
                {
                    result.AddRange(batchResult);
                }
                cancellationToken.ThrowIfCancellationRequested();
            }
            return result;
        }

        /// <summary>
        /// Returns chunks of 100 same table operations with the same entity partition
        /// </summary>
        private static IEnumerable<TableBatchOperation> GetLimitConformingBatches(IEnumerable<TableOperation> batch)
        {
            foreach (var sameOperationEntities in batch.GroupBy(x => x.OperationType))
            {
                foreach (var samePartitionEntities in sameOperationEntities.GroupBy(x => x.Entity.PartitionKey))
                {
                    foreach (var chunkOf100Entities in samePartitionEntities.ChunkBy(100))
                    {
                        var limitConformingBatch = new TableBatchOperation();
                        foreach (var entity in chunkOf100Entities)
                        {
                            limitConformingBatch.Add(entity);
                        }
                        yield return limitConformingBatch;
                    }
                }
            }
        }
    }
}
