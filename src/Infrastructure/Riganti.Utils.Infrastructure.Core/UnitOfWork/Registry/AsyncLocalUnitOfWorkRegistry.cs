using System.Collections.Generic;
using System.Threading;

namespace Riganti.Utils.Infrastructure.Core
{
    /// <summary>
    /// A unit of work storage which persists the unit of work instances in a AsyncLocal object.
    /// </summary>
    public class AsyncLocalUnitOfWorkRegistry : UnitOfWorkRegistryBase
    {

        private readonly AsyncLocal<Stack<IUnitOfWork>> stack = new AsyncLocal<Stack<IUnitOfWork>>();

        /// <summary>
        /// Gets the stack of currently active unit of work objects.
        /// </summary>
        protected override Stack<IUnitOfWork> GetStack()
        {
            if (stack.Value == null)
            {
                stack.Value = new Stack<IUnitOfWork>();
            }
            return stack.Value;
        }
    }
}