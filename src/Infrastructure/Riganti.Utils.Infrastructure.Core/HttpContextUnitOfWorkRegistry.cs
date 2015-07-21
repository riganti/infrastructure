using System.Collections.Generic;
using System.Web;

namespace Riganti.Utils.Infrastructure.Core
{
    /// <summary>
    /// A storage for unit of work objects which persists data in HttpContext.Items collection.
    /// </summary>
    public class HttpContextUnitOfWorkRegistry : UnitOfWorkRegistryBase
    {

        private readonly UnitOfWorkRegistryBase alternateRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpContextUnitOfWorkRegistry"/> class.
        /// </summary>
        /// <param name="alternateRegistry">An alternate storage that will be used for threads not associated with any HTTP request.</param>
        public HttpContextUnitOfWorkRegistry(UnitOfWorkRegistryBase alternateRegistry)
        {
            this.alternateRegistry = alternateRegistry;
        }

        /// <summary>
        /// Gets the stack of currently active unit of work objects.
        /// </summary>
        protected internal override Stack<IUnitOfWork> GetStack()
        {
            if (HttpContext.Current == null)
            {
                return alternateRegistry.GetStack();
            }
            else
            {
                var stack = HttpContext.Current.Items[typeof (HttpContextUnitOfWorkRegistry)] as Stack<IUnitOfWork>;
                if (stack == null)
                {
                    stack = new Stack<IUnitOfWork>();
                    HttpContext.Current.Items[typeof (HttpContextUnitOfWorkRegistry)] = stack;
                }
                return stack;
            }
        }
    }
}