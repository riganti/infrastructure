using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Reflection;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.EntityFrameworkCore
{
    public class EntityFrameworkFirstLevelQueryBase<TEntity> : IFirstLevelQuery<TEntity> where TEntity : class
    {
        private readonly IUnitOfWorkProvider unitOfWorkProvider;

        public EntityFrameworkFirstLevelQueryBase(IUnitOfWorkProvider unitOfWorkProvider)
        {
            this.unitOfWorkProvider = unitOfWorkProvider;
        }

        protected DbContext Context
        {
            get { return EntityFrameworkUnitOfWork.TryGetDbContext(unitOfWorkProvider); }
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