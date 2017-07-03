using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Reflection;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.EntityFrameworkCore
{
    /// <summary>
    /// A base class for first level queries which return filtered entity sets based on user identity or other criteria.
    /// </summary>
    public class EntityFrameworkFirstLevelQueryBase<TEntity> : EntityFrameworkFirstLevelQueryBase<TEntity, DbContext>
        where TEntity : class
    {
        public EntityFrameworkFirstLevelQueryBase(IUnitOfWorkProvider unitOfWorkProvider) : base(unitOfWorkProvider)
        {
        }
    }

    /// <summary>
    /// A base class for first level queries which return filtered entity sets based on user identity or other criteria.
    /// </summary>
    public class EntityFrameworkFirstLevelQueryBase<TEntity, TDbContext> : IFirstLevelQuery<TEntity>
        where TEntity : class
        where TDbContext : DbContext
    {
        private readonly IEntityFrameworkUnitOfWorkProvider<TDbContext> unitOfWorkProvider;

        public EntityFrameworkFirstLevelQueryBase(IEntityFrameworkUnitOfWorkProvider<TDbContext> unitOfWorkProvider)
        {
            this.unitOfWorkProvider = unitOfWorkProvider;
        }

        protected TDbContext Context
        {
            get
            {
                var context = EntityFrameworkUnitOfWork.TryGetDbContext<TDbContext>(unitOfWorkProvider);
                if (context == null)
                {
                    throw new InvalidOperationException("The EntityFrameworkRepository must be used in a unit of work of type EntityFrameworkUnitOfWork!");
                }
                return context;
            }
        }


        public virtual IQueryable<TEntity> GetEntitySet()
        {
            IQueryable<TEntity> set = Context.Set<TEntity>();

            if (typeof(ISoftDeleteEntity).GetTypeInfo().IsAssignableFrom(typeof(TEntity)))
            {
                set = set.Where(t => ((ISoftDeleteEntity) t).DeletedDate == null);
            }

            return set;
        }
    }
}