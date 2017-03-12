
using System;
using System.Collections.Generic;

namespace Riganti.Utils.Infrastructure.Azure.TableStorage
{
    /// <summary>
    /// Register mappings of type to cloud table.
    /// </summary>
    public class TableEntityMapperRegistry
    {
        private readonly Dictionary<Type, string> mappings;

        /// <summary>
        /// The registry instance.
        /// </summary>
        public static TableEntityMapperRegistry Instance => new Lazy<TableEntityMapperRegistry>(() => new TableEntityMapperRegistry()).Value;

        private TableEntityMapperRegistry()
        {
            mappings = new Dictionary<Type, string>();
        }

        /// <summary>
        /// Maps a type to cloud table.
        /// </summary>
        public void Map(Type type, string table)
        {
            mappings.Add(type, table);
        }

        /// <summary>
        /// Gets the mapped table for specified entity type.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>The mapped table name or name of the type as default.</returns>
        public string GetTable(Type entityType)
        {
            string table;
            if (!mappings.TryGetValue(entityType, out table))
                return Default(entityType);

            return table;
        }

        /// <summary>
        /// Gets the default type name.
        /// </summary>
        private static string Default(Type entityType)
        {
            return entityType.Name;
        }
    }
}
