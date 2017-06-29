using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Riganti.Utils.Infrastructure.Azure.TableStorage
{
    public class TableEntityMapperBase : ITableEntityMapper
    {
        /// <summary>
        /// Gets the mapping table for specified type.
        /// </summary>
        public virtual string GetTable(Type type)
        {
            return null;
        }

        /// <summary>
        /// Gets the mapping table for specified entity.
        /// </summary>
        public virtual string GetTable<TEntity>(TEntity entity) where TEntity : ITableEntity
        {
            return GetTable(entity.GetType());
        }

        /// <summary>
        /// Gets the mapping table for specified type.
        /// </summary>
        public string GetTable<TEntity>() where TEntity : ITableEntity
        {
            return GetTable(typeof(TEntity));
        }
    }
}
