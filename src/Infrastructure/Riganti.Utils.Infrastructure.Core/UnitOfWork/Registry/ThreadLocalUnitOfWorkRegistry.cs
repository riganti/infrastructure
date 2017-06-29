using System.Collections.Generic;
using System.Threading;

namespace Riganti.Utils.Infrastructure.Core
{
    /// <summary>
    /// A unit of work storage which persists the unit of work instances in a ThreadLocal object.
    /// </summary>
    public class ThreadLocalUnitOfWorkRegistry : UnitOfWorkRegistryBase
    {

        private readonly ThreadLocal<Stack<IUnitOfWork>> stack 
            = new ThreadLocal<Stack<IUnitOfWork>>(() => new Stack<IUnitOfWork>());

        /// <summary>
        /// Gets the stack of currently active unit of work objects.
        /// </summary>
        protected override Stack<IUnitOfWork> GetStack()
        {
            return stack.Value;
        }
    }
}