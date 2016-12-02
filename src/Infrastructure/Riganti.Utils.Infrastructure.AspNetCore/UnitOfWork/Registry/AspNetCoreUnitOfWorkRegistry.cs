using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.Core
{
    public class AspNetCoreUnitOfWorkRegistry : UnitOfWorkRegistryBase
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public AspNetCoreUnitOfWorkRegistry(IHttpContextAccessor httpContextAccessor, IUnitOfWorkRegistry alternateRegistry = null) : base(alternateRegistry)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        protected override Stack<IUnitOfWork> GetStack()
        {
            if (httpContextAccessor.HttpContext == null)
            {
                return null;
            }

            object result;
            if (!httpContextAccessor.HttpContext.Items.TryGetValue(typeof(AspNetCoreUnitOfWorkRegistry), out result))
            {
                result = new Stack<IUnitOfWork>();
                httpContextAccessor.HttpContext.Items[typeof(AspNetCoreUnitOfWorkRegistry)] = result;
            }
            return (Stack<IUnitOfWork>)result;
        }
    }
}
