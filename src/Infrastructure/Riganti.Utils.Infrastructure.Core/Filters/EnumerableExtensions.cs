using System;
using System.Collections.Generic;
using System.Linq;

namespace Riganti.Utils.Infrastructure.Core
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Filters a sequence of values based on the given filter.
        /// </summary>
        /// <typeparam name="TSource">The type of elements of source.</typeparam>
        /// <param name="source">An enumeration to filter.</param>
        /// <param name="filter">The filter to match against each element.</param>
        public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, FilterDTOBase filter)
        {
            return source.AsQueryable().Where(filter);
        }

        /// <summary>
        /// Filters a sequence of values based on the given filter.
        /// </summary>
        /// <typeparam name="TSource">The type of elements of source.</typeparam>
        /// <param name="source">An enumeration to filter.</param>
        /// <param name="filter">The filter to match against each element.</param>
        public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> source, FilterDTOBase filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (!filter.IsValid() || filter is FilterGroupDTO group && group.IsEmpty())
            {
                return source;
            }

            var builder = new FilterPredicateBuilder<TSource>();
            var predicate = builder.BuildPredicate(filter);

            return source.Where(predicate);
        }
    }
}