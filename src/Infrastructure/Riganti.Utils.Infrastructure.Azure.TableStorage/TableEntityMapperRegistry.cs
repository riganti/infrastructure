using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;

namespace Riganti.Utils.Infrastructure.Azure.TableStorage
{
    /// <summary>
    /// Register mappings of type to cloud table.
    /// </summary>
    public class TableEntityMapperRegistry : ITableEntityMapperRegistry
    {
        // contains collection of typeName-table
        private readonly Dictionary<string, string> mappings;

        public TableEntityMapperRegistry()
        {
            mappings = new Dictionary<string, string>();
        }

        /// <summary>
        /// Maps type to the specified table.
        /// </summary>
        public void Map(Type t, string table)
        {
            var name = t.Name;
            if (mappings.ContainsKey(name))
            {
                var map = mappings[name];
                if (map.Equals(table, StringComparison.OrdinalIgnoreCase))
                    throw new Exception("Mapping conflict! The specified type is already registered for another table.");

                return;
            }

            mappings.Add(name, table);
        }

        /// <summary>
        /// Maps type to the specified table.
        /// </summary>
        public void Map<TEntity>(string table) where TEntity : ITableEntity
        {
            Map(typeof(TEntity), table);
        }

        /// <summary>
        /// Gets the mapping table for specified type.
        /// </summary>
        public string GetTable(Type type)
        {
            string table;
            if (mappings.TryGetValue(type.Name, out table))
                return table;

            return type.Name;
        }

        /// <summary>
        /// Gets the mapping table for specified entity.
        /// </summary>
        public string GetTable<TEntity>(TEntity entity) where TEntity: ITableEntity
        {
            return GetTable(entity.GetType());
        }

        /// <summary>
        /// Gets the mapping table for specified type.
        /// </summary>
        public string GetTable<TEntity>() where TEntity: ITableEntity
        {
            return GetTable(typeof(TEntity));
        }
    }
}
