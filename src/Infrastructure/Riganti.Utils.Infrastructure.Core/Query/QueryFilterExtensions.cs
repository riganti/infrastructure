using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Riganti.Utils.Infrastructure.Core
{
    public static class QueryFilterExtensions
    {
        private static readonly MethodInfo containsMethod;
        private static readonly MethodInfo startsWithMethod;


        static QueryFilterExtensions()
        {
            containsMethod = typeof(string).GetTypeInfo().GetMethod("Contains", new [] { typeof(string) });
            startsWithMethod = typeof(string).GetTypeInfo().GetMethod("StartsWith", new [] { typeof(string) });
        }


        public static IQueryable<T> FilterOptionalString<T>(this IQueryable<T> data, Expression<Func<T, string>> fieldSelector, string valueToFilter, StringFilterMode mode)
        {
            if (string.IsNullOrEmpty(valueToFilter))
            {
                return data;
            }

            Expression body;
            switch (mode)
            {
                case StringFilterMode.Contains:
                    body = Expression.Call(fieldSelector.Body, containsMethod, Expression.Constant(valueToFilter));
                    break;

                case StringFilterMode.StartsWith:
                    body = Expression.Call(fieldSelector.Body, startsWithMethod, Expression.Constant(valueToFilter));
                    break;

                case StringFilterMode.Equals:
                    body = Expression.Equal(fieldSelector.Body, Expression.Constant(valueToFilter));
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
            return data.Where(Expression.Lambda<Func<T, bool>>(body, fieldSelector.Parameters));
        }

        public static IQueryable<T> FilterOptional<T, TValue>(this IQueryable<T> data, Expression<Func<T, TValue>> fieldSelector, TValue? valueToFilter) where TValue : struct
        {
            if (valueToFilter == null)
            {
                return data;
            }

            return FilterRequired(data, fieldSelector, valueToFilter.Value);
        }

        public static IQueryable<T> FilterOptional<T, TValue>(this IQueryable<T> data, Expression<Func<T, TValue?>> fieldSelector, TValue? valueToFilter) where TValue : struct
        {
            if (valueToFilter == null)
            {
                return data;
            }

            return FilterRequired(data, fieldSelector, valueToFilter);
        }

        public static IQueryable<T> FilterRequired<T, TValue>(this IQueryable<T> data, Expression<Func<T, TValue>> fieldSelector, TValue valueToFilter)
        {
            var body = Expression.Equal(fieldSelector.Body, Expression.Constant(valueToFilter, typeof(TValue)));
            return data.Where(Expression.Lambda<Func<T, bool>>(body, fieldSelector.Parameters));
        }

    }

}
