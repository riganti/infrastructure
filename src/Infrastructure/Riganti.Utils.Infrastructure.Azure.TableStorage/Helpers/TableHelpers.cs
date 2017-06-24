using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Riganti.Utils.Infrastructure.Azure.TableStorage.Helpers
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
    }
}
