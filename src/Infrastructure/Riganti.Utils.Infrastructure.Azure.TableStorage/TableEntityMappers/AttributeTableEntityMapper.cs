using System;
using System.Reflection;

namespace Riganti.Utils.Infrastructure.Azure.TableStorage
{
    /// <summary>
    /// Uses <see cref="TableNameAttribute"/> to determine a name of table.
    /// </summary>
    public class AttributeTableEntityMapper : TableEntityMapperBase
    {
        public override string GetTable(Type type)
        {
            var attribute = type.GetTypeInfo().GetCustomAttribute<TableNameAttribute>();
            return attribute?.Name;
        }
    }
}
