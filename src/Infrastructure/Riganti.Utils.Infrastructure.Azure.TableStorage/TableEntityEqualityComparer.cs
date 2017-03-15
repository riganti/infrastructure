using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;

namespace Riganti.Utils.Infrastructure.Azure.TableStorage
{
    /// <summary>
    /// The comparer for TableEntity which uniqueness within table is represented by both PartitionKey and RowKey.
    /// </summary>
    public class TableEntityEqualityComparer<TEntity> : IEqualityComparer, IEqualityComparer<TEntity> where TEntity: class, ITableEntity
    {
        public bool Equals(TEntity entity1, TEntity entity2)
        {
            if (entity1 == null && entity2 == null)
                return true;

            if (entity1 == null || entity2 == null)
                return false;

            return
                entity1.PartitionKey.Equals(entity2.PartitionKey) 
                && entity1.RowKey.Equals(entity2.RowKey);
        }

        public int GetHashCode(TEntity entity)
        {
            unchecked
            {
                return (entity.PartitionKey?.GetHashCode() ?? 0) 
                    ^ (entity.RowKey?.GetHashCode() ?? 0);
            }
        }

        public new bool Equals(object x, object y)
        {
            var entity1 = x as TEntity;
            if (entity1 == null) return false;

            var entity2 = y as TEntity;
            if (entity2 == null) return false;

            return
                entity1.PartitionKey.Equals(entity2.PartitionKey)
                && entity1.RowKey.Equals(entity2.RowKey);
        }

        public int GetHashCode(object obj)
        {
            unchecked
            {
                var entity = (TEntity) obj;
                return (entity.PartitionKey?.GetHashCode() ?? 0)
                    ^ (entity.RowKey?.GetHashCode() ?? 0);
            }
        }
    }
}
