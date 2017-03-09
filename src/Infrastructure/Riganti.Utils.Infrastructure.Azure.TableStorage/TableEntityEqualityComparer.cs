using System;
using System.Collections.Generic;

namespace Riganti.Utils.Infrastructure.Azure.TableStorage
{
    /// <summary>
    /// The comparer for TableEntity which uniqueness within table is represented by both PartitionKey and RowKey.
    /// </summary>
    public class TableEntityEqualityComparer : IEqualityComparer<Tuple<string,string>>
    {
        public bool Equals(Tuple<string,string> entity1, Tuple<string,string> entity2)
        {
            if (entity1 == null && entity2 == null)
                return true;

            if (entity1 == null || entity2 == null)
                return false;

            return entity1.Equals(entity2);

        }

        public int GetHashCode(Tuple<string,string> entity)
        {
            unchecked
            {
                return ((entity.Item1?.GetHashCode() ?? 0) * 397) ^ (entity.Item2?.GetHashCode() ?? 0);
            }
        }
    }
}
