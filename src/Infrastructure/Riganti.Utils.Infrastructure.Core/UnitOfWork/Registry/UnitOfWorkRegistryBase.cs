using System;
using System.Collections.Generic;
using System.Linq;

namespace Riganti.Utils.Infrastructure.Core
{
    /// <summary>
    /// A base implementation of unit of work registry.
    /// </summary>
    public abstract class UnitOfWorkRegistryBase : IUnitOfWorkRegistry
    {
        private readonly IUnitOfWorkRegistry alternateRegistry;

        /// <summary>
        /// Gets the alternate registry and throws an exception when there is no such registry configured.
        /// </summary>
        protected IUnitOfWorkRegistry AlternateRegistry
        {
            get
            {
                if (alternateRegistry == null)
                {
                    throw new InvalidOperationException($"The {GetType()} was not able to provide current unit of work and there is no alternate registry configured!");
                }
                return alternateRegistry;
            }
        }

        public UnitOfWorkRegistryBase(IUnitOfWorkRegistry alternateRegistry = null)
        {
            this.alternateRegistry = alternateRegistry;
        }


        /// <summary>
        /// Gets the stack of currently active unit of work objects.
        /// If the registry is unable to provide such stack, it should return null to let the caller to use alternate registry.
        /// </summary>
        protected abstract Stack<IUnitOfWork> GetStack();


        /// <summary>
        /// Registers a new unit of work.
        /// </summary>
        public void RegisterUnitOfWork(IUnitOfWork unitOfWork)
        {
            var unitOfWorkStack = GetStack();
            if (unitOfWorkStack == null)
            {
                AlternateRegistry.RegisterUnitOfWork(unitOfWork);
            }
            else
            {
                unitOfWorkStack.Push(unitOfWork);
            }
        }

        /// <summary>
        /// Unregisters a specified unit of work.
        /// </summary>
        public void UnregisterUnitOfWork(IUnitOfWork unitOfWork)
        {
            var unitOfWorkStack = GetStack();
            if (unitOfWorkStack == null)
            {
                AlternateRegistry.UnregisterUnitOfWork(unitOfWork);
            }
            else
            {
                if (unitOfWorkStack.Any())
                {
                    if (unitOfWorkStack.Pop() == unitOfWork)
                    {
                        return;
                    }
                }
                throw new InvalidOperationException("Some of the unit of works was not disposed correctly!");
            }
        }

        /// <summary>
        /// Gets the unit of work in the current scope.
        /// </summary>
        public IUnitOfWork GetCurrent(int ancestorLevel = 0)
        {
            var unitOfWorkStack = GetStack();
            if (unitOfWorkStack == null)
            {
                return AlternateRegistry.GetCurrent(ancestorLevel);
            }

            if (ancestorLevel >= unitOfWorkStack.Count)
            {
                return null;
            }
            else if (ancestorLevel == 0)
            {
                return unitOfWorkStack.Peek();
            }
            else 
            {
                return unitOfWorkStack.ToArray()[ancestorLevel];
            }
        }
    }
}