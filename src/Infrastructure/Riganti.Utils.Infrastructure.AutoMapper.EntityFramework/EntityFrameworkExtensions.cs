using AutoMapper;
using System.Data.Entity;
using Riganti.Utils.Infrastructure.Core;
using Riganti.Utils.Infrastructure.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Riganti.Utils.Infrastructure.AutoMapper
{
    public static class EntityFrameworkCoreExtensions
    {

        public static void DropAndCreateCollection<TDbContext, TSource, TSourceItem, TDestination, TDestinationItem>
            (
                this IMemberConfigurationExpression<TSource, TDestination, ICollection<TDestinationItem>> config,
                IEntityFrameworkUnitOfWorkProvider<TDbContext> unitOfWorkProvider,
                Expression<Func<TSource, ICollection<TSourceItem>>> sourceCollectionSelector,
                Func<TSourceItem, TDestinationItem> projection = null,
                Action<TDestinationItem> removeCallback = null,
                Func<TDestinationItem, bool> destinationFilter = null
            )
            where TDbContext : DbContext
            where TDestinationItem : class
        {
            if (removeCallback == null)
            {
                removeCallback = item =>
                {
                    var uow = EntityFrameworkUnitOfWork.TryGetDbContext<TDbContext>(unitOfWorkProvider);
                    uow.Set<TDestinationItem>().Remove(item);
                };
            }

            Extensions.DropAndCreateCollection(config, sourceCollectionSelector, projection, removeCallback, destinationFilter);
        }


        public static void SyncCollectionByKey<TDbContext, TSource, TSourceItem, TDestination, TDestinationItem>
            (
                this IMemberConfigurationExpression<TSource, TDestination, ICollection<TDestinationItem>> config,
                IEntityFrameworkUnitOfWorkProvider<TDbContext> unitOfWorkProvider,
                Expression<Func<TSource, ICollection<TSourceItem>>> sourceCollectionSelector,
                Func<TSourceItem, TDestinationItem> createFunction = null,
                Action<TSourceItem, TDestinationItem> updateFunction = null,
                Action<TDestinationItem> removeFunction = null,
                Func<TDestinationItem, bool> destinationFilter = null
            )
            where TDbContext : DbContext
            where TDestinationItem : class
        {
            if (removeFunction == null)
            {
                removeFunction = d =>
                {
                    var dbContext = EntityFrameworkUnitOfWork.TryGetDbContext(unitOfWorkProvider);
                    dbContext.Set<TDestinationItem>().Remove(d);
                };
            }

            var sourceKeyEntityType = typeof(TSourceItem).GetTypeInfo()
                .GetInterfaces()
                .Select(s => s.GetTypeInfo())
                .SingleOrDefault(s => s.IsGenericType && s.GetGenericTypeDefinition() == typeof(IEntity<>));
            var sourceKeyType = sourceKeyEntityType?.GetGenericArguments()[0];

            var destinationKeyEntityType = typeof(TDestinationItem).GetTypeInfo()
                .GetInterfaces()
                .Select(s => s.GetTypeInfo())
                .SingleOrDefault(s => s.IsGenericType && s.GetGenericTypeDefinition() == typeof(IEntity<>));
            var destinationKeyType = destinationKeyEntityType?.GetGenericArguments()[0];

            if (sourceKeyType == null || destinationKeyType == null || sourceKeyType != destinationKeyType)
            {
                throw new InvalidOperationException("The source and destination collection items must implement the IEntity<TKey> interface and the keys must be equal!");
            }

            var sourceParam = Expression.Parameter(typeof(TSourceItem), "i");
            var sourceKeySelector = Expression.Lambda(Expression.Property(sourceParam, nameof(IEntity<int>.Id)), sourceParam);
            var destinationParam = Expression.Parameter(typeof(TDestinationItem), "i");
            var destinationKeySelector = Expression.Lambda(Expression.Property(destinationParam, nameof(IEntity<int>.Id)), destinationParam);

            var method = typeof(Extensions).GetTypeInfo().GetMethod("SyncCollectionByKeyReflectionOnly", BindingFlags.NonPublic | BindingFlags.Static);
            method.MakeGenericMethod(typeof(TSource), typeof(TSourceItem), typeof(TDestination), typeof(TDestinationItem), sourceKeyType)
                .Invoke(null, new object[] { config, sourceCollectionSelector, sourceKeySelector, destinationKeySelector, createFunction, updateFunction, removeFunction, true, destinationFilter });
        }


        public static void SyncCollectionByKey<TDbContext, TSource, TSourceItem, TDestination, TDestinationItem, TKey>
            (
                this IMemberConfigurationExpression<TSource, TDestination, ICollection<TDestinationItem>> config,
                IEntityFrameworkUnitOfWorkProvider<TDbContext> unitOfWorkProvider,
                Expression<Func<TSource, ICollection<TSourceItem>>> sourceCollectionSelector,
                Func<TSourceItem, TKey> sourceKeySelector,
                Func<TDestinationItem, TKey> destinationKeySelector,
                Func<TSourceItem, TDestinationItem> createFunction = null,
                Action<TSourceItem, TDestinationItem> updateFunction = null,
                Action<TDestinationItem> removeFunction = null,
                Func<TDestinationItem, bool> destinationFilter = null
            )
            where TDbContext : DbContext
            where TDestinationItem : class
        {
            if (removeFunction == null)
            {
                removeFunction = d =>
                {
                    var uow = EntityFrameworkUnitOfWork.TryGetDbContext(unitOfWorkProvider);
                    uow.Set<TDestinationItem>().Remove(d);
                };
            }

            Extensions.SyncCollectionByKey(config, sourceCollectionSelector, sourceKeySelector, destinationKeySelector, createFunction, updateFunction, removeFunction, true, destinationFilter);
        }

    }
}
