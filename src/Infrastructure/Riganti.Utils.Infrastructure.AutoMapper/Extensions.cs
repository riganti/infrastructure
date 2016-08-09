using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Riganti.Utils.Infrastructure.Core;
using Riganti.Utils.Infrastructure.EntityFramework;

namespace Riganti.Utils.Infrastructure.AutoMapper
{
    public static class Extensions
    {



        public static void DropAndCreateCollection<TSource, TSourceItem, TDestination, TDestinationItem>
            (
                this IMemberConfigurationExpression<TSource, TDestination, ICollection<TDestinationItem>> config,
                Expression<Func<TSource, ICollection<TSourceItem>>> sourceCollectionSelector,
                Func<TSourceItem, TDestinationItem> projection = null,
                Action<TDestinationItem> removeCallback = null
            )
        {
            config.ResolveUsing(new DropAndCreateCollectionResolver<TSource, TSourceItem, TDestination, TDestinationItem>(projection, removeCallback), sourceCollectionSelector);
        }

        public static void DropAndCreateCollection<TSource, TSourceItem, TDestination, TDestinationItem>
            (
                this IMemberConfigurationExpression<TSource, TDestination, ICollection<TDestinationItem>> config,
                IUnitOfWorkProvider unitOfWorkProvider,
                Expression<Func<TSource, ICollection<TSourceItem>>> sourceCollectionSelector,
                Func<TSourceItem, TDestinationItem> projection = null,
                Action<TDestinationItem> removeCallback = null
            )
            where TDestinationItem : class
        {
            if (removeCallback == null)
            {
                removeCallback = item =>
                {
                    var uow = EntityFrameworkUnitOfWork.TryGetDbContext(unitOfWorkProvider);
                    uow.Set<TDestinationItem>().Remove(item);
                };
            }

            DropAndCreateCollection(config, sourceCollectionSelector, projection, removeCallback);
        }


        private static void SyncCollectionByKeyReflectionOnly<TSource, TSourceItem, TDestination, TDestinationItem, TKey>
            (
                this IMemberConfigurationExpression<TSource, TDestination, ICollection<TDestinationItem>> config,
                Expression<Func<TSource, ICollection<TSourceItem>>> sourceCollectionSelector,
                Expression<Func<TSourceItem, TKey>> sourceKeySelector,
                Expression<Func<TDestinationItem, TKey>> destinationSelector,
                Func<TSourceItem, TDestinationItem> createFunction = null,
                Action<TSourceItem, TDestinationItem> updateFunction = null,
                Action<TSourceItem, TDestinationItem> removeFunction = null,
                bool keepRemovedItemsInDestinationCollection = false
            )
        {
            SyncCollectionByKey(config, sourceCollectionSelector, sourceKeySelector.Compile(), destinationSelector.Compile(), createFunction, updateFunction, removeFunction, keepRemovedItemsInDestinationCollection);
        }


        public static void SyncCollectionByKey<TSource, TSourceItem, TDestination, TDestinationItem, TKey>
            (
                this IMemberConfigurationExpression<TSource, TDestination, ICollection<TDestinationItem>> config,
                Expression<Func<TSource, ICollection<TSourceItem>>> sourceCollectionSelector,
                Func<TSourceItem, TKey> sourceKeySelector,
                Func<TDestinationItem, TKey> destinationSelector,
                Func<TSourceItem, TDestinationItem> createFunction = null,
                Action<TSourceItem, TDestinationItem> updateFunction = null,
                Action<TSourceItem, TDestinationItem> removeFunction = null,
                bool keepRemovedItemsInDestinationCollection = false
            )
        {
            config.ResolveUsing(new SyncByKeyCollectionResolver<TSource, TSourceItem, TDestination, TDestinationItem, TKey>()
            {
                SourceKeySelector = sourceKeySelector,
                DestinationKeySelector = destinationSelector,
                CreateFunction = createFunction ?? Mapper.Map<TSourceItem, TDestinationItem>,
                UpdateFunction = updateFunction ?? ((s, d) => Mapper.Map(s, d)),
                RemoveFunction = removeFunction ?? ((s, d) => { }),
                KeepRemovedItemsInDestinationCollection = keepRemovedItemsInDestinationCollection
            }, sourceCollectionSelector);
        }

        public static void SyncCollectionByKey<TSource, TSourceItem, TDestination, TDestinationItem>
            (
                this IMemberConfigurationExpression<TSource, TDestination, ICollection<TDestinationItem>> config,
                IUnitOfWorkProvider unitOfWorkProvider,
                Expression<Func<TSource, ICollection<TSourceItem>>> sourceCollectionSelector,
                Func<TSourceItem, TDestinationItem> createFunction = null,
                Action<TSourceItem, TDestinationItem> updateFunction = null,
                Action<TSourceItem, TDestinationItem> removeFunction = null
            )
            where TDestinationItem : class
        {
            if (removeFunction == null)
            {
                removeFunction = (s, d) =>
                {
                    var uow = EntityFrameworkUnitOfWork.TryGetDbContext(unitOfWorkProvider);
                    uow.Set<TDestinationItem>().Remove(d);
                };
            }

            var sourceKeyEntityType = typeof(TSourceItem).GetInterfaces().SingleOrDefault(s => s.IsGenericType && s.GetGenericTypeDefinition() == typeof(IEntity<>));
            var sourceKeyType = sourceKeyEntityType?.GetGenericArguments()[0];

            var destinationKeyEntityType = typeof(TDestinationItem).GetInterfaces().SingleOrDefault(s => s.IsGenericType && s.GetGenericTypeDefinition() == typeof(IEntity<>));
            var destinationKeyType = destinationKeyEntityType?.GetGenericArguments()[0];

            if (sourceKeyType == null || destinationKeyType == null || sourceKeyType != destinationKeyType)
            {
                throw new InvalidOperationException("The source and destination collection items must implement the IEntity<TKey> interface and the keys must be equal!");
            }

            var sourceParam = Expression.Parameter(typeof(TSourceItem), "i");
            var sourceKeySelector = Expression.Lambda(Expression.Property(sourceParam, nameof(IEntity<int>.Id)), sourceParam);
            var destinationParam = Expression.Parameter(typeof(TDestinationItem), "i");
            var destinationKeySelector = Expression.Lambda(Expression.Property(destinationParam, nameof(IEntity<int>.Id)), destinationParam);

            var method = typeof(Extensions).GetMethod("SyncCollectionByKeyReflectionOnly", BindingFlags.NonPublic | BindingFlags.Static);
            method.MakeGenericMethod(typeof(TSource), typeof(TSourceItem), typeof(TDestination), typeof(TDestinationItem), sourceKeyType)
                .Invoke(null, new object[] { config, sourceCollectionSelector, sourceKeySelector, destinationKeySelector, createFunction, updateFunction, removeFunction, true });
        }


        public static void SyncCollectionByKey<TSource, TSourceItem, TDestination, TDestinationItem, TKey>
            (
                this IMemberConfigurationExpression<TSource, TDestination, ICollection<TDestinationItem>> config,
                IUnitOfWorkProvider unitOfWorkProvider,
                Expression<Func<TSource, ICollection<TSourceItem>>> sourceCollectionSelector,
                Func<TSourceItem, TKey> sourceKeySelector,
                Func<TDestinationItem, TKey> destinationKeySelector,
                Func<TSourceItem, TDestinationItem> createFunction = null,
                Action<TSourceItem, TDestinationItem> updateFunction = null,
                Action<TSourceItem, TDestinationItem> removeFunction = null
            )
            where TDestinationItem : class
        {
            if (removeFunction == null)
            {
                removeFunction = (s, d) =>
                {
                    var uow = EntityFrameworkUnitOfWork.TryGetDbContext(unitOfWorkProvider);
                    uow.Set<TDestinationItem>().Remove(d);
                };
            }

            SyncCollectionByKey(config, sourceCollectionSelector, sourceKeySelector, destinationKeySelector, createFunction, updateFunction, removeFunction, true);
        }
    }
}
