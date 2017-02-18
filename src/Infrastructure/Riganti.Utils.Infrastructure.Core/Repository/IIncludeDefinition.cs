using System;
using System.Linq;
using System.Linq.Expressions;

namespace Riganti.Utils.Infrastructure.Core
{
    public interface IIncludeDefinition<T>
    {

        IQueryable<T> ApplyInclude(IQueryable<T> query);

    }
}