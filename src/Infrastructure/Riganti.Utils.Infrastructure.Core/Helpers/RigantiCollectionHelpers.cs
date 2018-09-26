using System.Collections.Generic;
using System.Linq;

namespace Riganti.Utils.Infrastructure.Core
{
    public static class RigantiCollectionHelpers
    {
        public static IEnumerable<IEnumerable<T>> ChunkBy<T>(this IEnumerable<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value));
        }
    }
}