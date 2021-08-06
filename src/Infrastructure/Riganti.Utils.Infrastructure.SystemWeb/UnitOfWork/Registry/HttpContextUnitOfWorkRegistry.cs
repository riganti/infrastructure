using System.Collections.Generic;
using System.Web;

// ReSharper disable once CheckNamespace
namespace Riganti.Utils.Infrastructure.Core
{
    /// <summary>
    /// A storage for unit of work objects which persists data in HttpContext.Items collection.
    /// </summary>
    public class HttpContextUnitOfWorkRegistry : UnitOfWorkRegistryBase
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpContextUnitOfWorkRegistry"/> class.
        /// </summary>
        /// <param name="alternateRegistry">An alternate storage that will be used for threads not associated with any HTTP request.</param>
        public HttpContextUnitOfWorkRegistry(UnitOfWorkRegistryBase alternateRegistry) : base(alternateRegistry)
        {
        }

        /// <summary>
        /// Gets the stack of currently active unit of work objects.
        /// </summary>
        protected override Stack<IUnitOfWork> GetStack()
        {
            if (HttpContext.Current == null)
            {
                return null;
            }
            else
            {
                var stack = HttpContext.Current.Items[typeof(HttpContextUnitOfWorkRegistry)] as Stack<IUnitOfWork>;
                if (stack == null)
                {
                    stack = new Stack<IUnitOfWork>();
                    HttpContext.Current.Items[typeof(HttpContextUnitOfWorkRegistry)] = stack;
                }
                return stack;
            }
        }
    }
}