using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Internal.Linq;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.EntityFramework.Tests.Repository
{
    internal static class DbContextMockExtensions
    {
        internal static Mock<IDbSet<TEntity>> SetupDbSet<TDbContext, TEntity, TKey>(this Mock<TDbContext> dbContextMock, IList<TEntity> data, Expression<Func<TDbContext, IDbSet<TEntity>>> dbSetSelector = null)
            where TDbContext : DbContext
            where TEntity : class, IEntity<TKey>, new()
        {
            var dbSetMock = CreateDbSetMock<TEntity, TKey>(data);

            var dbSet = dbSetMock.Object.CastTo<DbSet<TEntity>>();
            if (dbSetSelector != null)
            {
                dbContextMock.Setup(dbSetSelector).Returns(dbSet);
            }
            dbContextMock.Setup(m => m.Set<TEntity>()).Returns(dbSet);
            return dbSetMock;
        }
        
        private static Mock<IDbSet<TEntity>> CreateDbSetMock<TEntity, TKey>(IList<TEntity> data)
            where TEntity : class, IEntity<TKey>, new()
        {
            var queryableData = data.AsQueryable();

            var dbSetMock = new Mock<TestDbSet<TEntity, TKey>> (data) {CallBase = true};
            dbSetMock.As<IDbAsyncEnumerable<TEntity>>()
                .Setup(m => m.GetAsyncEnumerator())
                .Returns(new TestDbAsyncEnumerator<TEntity>(queryableData.GetEnumerator()));

            dbSetMock.As<IQueryable<TEntity>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<TEntity>(queryableData.Provider));
            dbSetMock.As<IQueryable<TEntity>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            dbSetMock.As<IQueryable<TEntity>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            dbSetMock.As<IQueryable<TEntity>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());
            
            return dbSetMock.As<IDbSet<TEntity>>();
        }

        private class TestDbAsyncQueryProvider<TEntity> : IDbAsyncQueryProvider
        {
            private readonly IQueryProvider inner;

            internal TestDbAsyncQueryProvider(IQueryProvider inner)
            {
                this.inner = inner;
            }

            public IQueryable CreateQuery(Expression expression)
            {
                return new TestDbAsyncEnumerable<TEntity>(expression);
            }

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            {
                return new TestDbAsyncEnumerable<TElement>(expression);
            }

            public object Execute(Expression expression)
            {
                return inner.Execute(expression);
            }

            public TResult Execute<TResult>(Expression expression)
            {
                return inner.Execute<TResult>(expression);
            }

            public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
            {
                return Task.FromResult(Execute(expression));
            }

            public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
            {
                return Task.FromResult(Execute<TResult>(expression));
            }
        }

        private class TestDbAsyncEnumerable<T> : EnumerableQuery<T>, IDbAsyncEnumerable<T>, IQueryable<T>
        {
            public TestDbAsyncEnumerable(IEnumerable<T> enumerable)
                : base(enumerable)
            {
            }

            public TestDbAsyncEnumerable(Expression expression)
                : base(expression)
            {
            }

            public IDbAsyncEnumerator<T> GetAsyncEnumerator()
            {
                return new TestDbAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
            }

            IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
            {
                return GetAsyncEnumerator();
            }

            IQueryProvider IQueryable.Provider => new TestDbAsyncQueryProvider<T>(this);
        }

        private class TestDbAsyncEnumerator<T> : IDbAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> inner;

            public TestDbAsyncEnumerator(IEnumerator<T> inner)
            {
                this.inner = inner;
            }

            public void Dispose()
            {
                inner.Dispose();
            }

            public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
            {
                return Task.FromResult(inner.MoveNext());
            }

            public T Current => inner.Current;

            object IDbAsyncEnumerator.Current => Current;
        }
    }

    public class TestDbSet<TEntity, TKey> : DbSet<TEntity>, IDbSet<TEntity>
            where TEntity : class, IEntity<TKey>, new()
  {
    public override ObservableCollection<TEntity> Local { get; }
    IQueryable LocalQueryable => Local.AsQueryable();

        public TestDbSet() : this(new List<TEntity>())
        {
        }

        public TestDbSet(IEnumerable<TEntity> data)
        {
            Local = new ObservableCollection<TEntity>(data);
        }

        public override TEntity Find(params object[] keyValues)
        {
            throw new NotImplementedException("Derive from TestDbSet<T> and override Find");
        }

        public override TEntity Add(TEntity item)
        {
            Local.Add(item);
            return item;
        }

        public override TEntity Remove(TEntity item)
        {
          var wasRemoved = Local.Remove(item);
          if (!wasRemoved)
          {
            var itemWithSameId = Local.SingleOrDefault(i => Equals(i.Id, item.Id));
            if (itemWithSameId != null)
            {
              Local.Remove(itemWithSameId);
            }
          }
          return item;
        }

        public override TEntity Attach(TEntity item)
        {
            var itemWithSameId = Local.SingleOrDefault(i => Equals(i.Id, item.Id));
            if (itemWithSameId == null)
            {
                Local.Add(item);
            }
            return item;
        }

        public override TEntity Create()
        {
            return new TEntity();
        }

        public override TDerivedEntity Create<TDerivedEntity>()
        {
            return Activator.CreateInstance<TDerivedEntity>();
        }
        
        Type IQueryable.ElementType => LocalQueryable.ElementType;

        Expression IQueryable.Expression => LocalQueryable.Expression;

        IQueryProvider IQueryable.Provider => LocalQueryable.Provider;

        IEnumerator IEnumerable.GetEnumerator() => Local.GetEnumerator();

        IEnumerator<TEntity> IEnumerable<TEntity>.GetEnumerator() => Local.GetEnumerator();
    }
}