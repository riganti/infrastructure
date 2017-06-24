using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.Azure.TableStorage
{
    public class BatchSafeCloudTable : CloudTable
    {
        public BatchSafeCloudTable(Uri tableAddress) : base(tableAddress)
        {
        }

        public BatchSafeCloudTable(Uri tableAbsoluteUri, StorageCredentials credentials) : base(tableAbsoluteUri, credentials)
        {
        }

        public BatchSafeCloudTable(StorageUri tableAddress, StorageCredentials credentials) : base(tableAddress, credentials)
        {
        }

        public async Task<IList<TableResult>> ExecuteBatchSafeAsync(TableBatchOperation batch, TableRequestOptions requestOptions, OperationContext operationContext,
            CancellationToken cancellationToken, int numberOfBatchesProcesedInParallel = 3)
        {
            var result = new List<TableResult>();

            foreach (var chunk in GetLimitConformingBatches(batch).ToList().ChunkBy(numberOfBatchesProcesedInParallel))
            {
                foreach (var batchResult in await Task.WhenAll(chunk.Select(b => ExecuteBatchAsync(b, requestOptions, operationContext, cancellationToken))))
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
        private IEnumerable<TableBatchOperation> GetLimitConformingBatches(TableBatchOperation batch)
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
