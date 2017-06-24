using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riganti.Utils.Infrastructure.Azure.TableStorage.TableEntityMappers
{
    /// <summary>
    /// Evaluates results of multiple implementations of ITableEntityMapper. Returns first non-null result.
    /// </summary>
    public class AggregateTableEntityMapper : TableEntityMapperBase
    {
        private readonly ITableEntityMapper[] mappers;

        public AggregateTableEntityMapper(params ITableEntityMapper[] mappers)
        {
            this.mappers = mappers;
        }

        public override string GetTable(Type type)
        {
            foreach (var mapper in mappers)
            {
                var name = mapper.GetTable(type);
                if (name != null) return name;
            }
            throw new Exception("Could not map the entity to a table name.");
        }
    }
}
