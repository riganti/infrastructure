using System;
using System.Collections.Generic;

namespace Riganti.Utils.Infrastructure.Core
{
    /// <summary>
    /// A base implementation of unit of work registry.
    /// </summary>
    public abstract class UnitOfWorkRegistryBase : IUnitOfWorkRegistry
    {

        /// <summary>
        /// Gets the stack of currently active unit of work objects.
        /// </summary>
        protected internal abstract Stack<IUnitOfWork> GetStack();

        /// <summary>
        /// Registers a new unit of work.
        /// </summary>
        public void RegisterUnitOfWork(IUnitOfWork unitOfWork)
        {
            GetStack().Push(unitOfWork);
        }

        /// <summary>
        /// Unregisters a specified unit of work.
        /// </summary>
        public void UnregisterUnitOfWork(IUnitOfWork unitOfWork)
        {
            if (GetStack().Pop() != unitOfWork)
            {
                throw new InvalidOperationException("Some of the unit of works was not disposed correctly!");
            }
        }

        /// <summary>
        /// Gets the unit of work in the current scope.
        /// </summary>
        public IUnitOfWork GetCurrent()
        {
            if (GetStack().Count == 0)
            {
                return null;
            }
            else
            {
                return GetStack().Peek();
            }
        }

    }
}