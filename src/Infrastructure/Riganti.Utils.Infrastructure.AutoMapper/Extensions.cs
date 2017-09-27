using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AutoMapper;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.AutoMapper
{
    public static class Extensions
    {

        public static void DropAndCreateCollection<TSource, TSourceItem, TDestination, TDestinationItem>
            (
                this IMemberConfigurationExpression<TSource, TDestination, ICollection<TDestinationItem>> config,
                Expression<Func<TSource, ICollection<TSourceItem>>> sourceCollectionSelector,
                Func<TSourceItem, TDestinationItem> projection = null,
                Action<TDestinationItem> removeCallback = null,
                Func<TDestinationItem, bool> destinationFilter = null
            )
        {
            config.ResolveUsing(new DropAndCreateCollectionResolver<TSource, TSourceItem, TDestination, TDestinationItem>(projection, removeCallback, destinationFilter), sourceCollectionSelector);
        }

        private static void SyncCollectionByKeyReflectionOnly<TSource, TSourceItem, TDestination, TDestinationItem, TKey>
            (
                this IMemberConfigurationExpression<TSource, TDestination, ICollection<TDestinationItem>> config,
                Expression<Func<TSource, ICollection<TSourceItem>>> sourceCollectionSelector,
                Expression<Func<TSourceItem, TKey>> sourceKeySelector,
                Expression<Func<TDestinationItem, TKey>> destinationSelector,
                Func<TSourceItem, TDestinationItem> createFunction = null,
                Action<TSourceItem, TDestinationItem> updateFunction = null,
                Action<TDestinationItem> removeFunction = null,
                bool keepRemovedItemsInDestinationCollection = false,
                Func<TDestinationItem, bool> destinationFilter = null
            )
        {
            SyncCollectionByKey(config, sourceCollectionSelector, sourceKeySelector.Compile(), destinationSelector.Compile(), createFunction, updateFunction, removeFunction, keepRemovedItemsInDestinationCollection, destinationFilter);
        }


        public static void SyncCollectionByKey<TSource, TSourceItem, TDestination, TDestinationItem, TKey>
            (
                this IMemberConfigurationExpression<TSource, TDestination, ICollection<TDestinationItem>> config,
                Expression<Func<TSource, ICollection<TSourceItem>>> sourceCollectionSelector,
                Func<TSourceItem, TKey> sourceKeySelector,
                Func<TDestinationItem, TKey> destinationSelector,
                Func<TSourceItem, TDestinationItem> createFunction = null,
                Action<TSourceItem, TDestinationItem> updateFunction = null,
                Action<TDestinationItem> removeFunction = null,
                bool keepRemovedItemsInDestinationCollection = false,
                Func<TDestinationItem, bool> destinationFilter = null
            )
        {
            config.ResolveUsing(new SyncByKeyCollectionResolver<TSource, TSourceItem, TDestination, TDestinationItem, TKey>()
            {
                SourceKeySelector = sourceKeySelector,
                DestinationKeySelector = destinationSelector,
                CreateFunction = createFunction ?? Mapper.Map<TSourceItem, TDestinationItem>,
                UpdateFunction = updateFunction ?? ((s, d) => Mapper.Map(s, d)),
                RemoveFunction = removeFunction ?? (d => { }),
                KeepRemovedItemsInDestinationCollection = keepRemovedItemsInDestinationCollection,
                DestinationFilter = destinationFilter ?? (e => true)
            }, sourceCollectionSelector);
        }

    }
}
