using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using AutoMapper;

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
            var dropAndCreateCollectionResolver = new DropAndCreateCollectionResolver<TSource, TSourceItem, TDestination, TDestinationItem>(projection, removeCallback, destinationFilter);
            config.MapFrom(dropAndCreateCollectionResolver, sourceCollectionSelector);
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
            config.MapFrom(new SyncByKeyCollectionResolver<TSource, TSourceItem, TDestination, TDestinationItem, TKey>()
            {
                SourceKeySelector = sourceKeySelector,
                DestinationKeySelector = destinationSelector,
                CreateFunction = createFunction,
                UpdateFunction = updateFunction,
                RemoveFunction = removeFunction,
                KeepRemovedItemsInDestinationCollection = keepRemovedItemsInDestinationCollection,
                DestinationFilter = destinationFilter
            }, sourceCollectionSelector);
        }

    }
}
