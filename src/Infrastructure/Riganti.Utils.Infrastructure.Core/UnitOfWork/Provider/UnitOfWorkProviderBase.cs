using System;

namespace Riganti.Utils.Infrastructure.Core
{
    /// <summary>
    /// A base implementation of unit of work provider.
    /// </summary>
    public abstract class UnitOfWorkProviderBase : IUnitOfWorkProvider
    {
        private readonly IUnitOfWorkRegistry registry;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkProviderBase"/> class.
        /// </summary>
        protected UnitOfWorkProviderBase(IUnitOfWorkRegistry registry)
        {
            this.registry = registry;
        }

        /// <summary>
        /// Creates a new unit of work.
        /// </summary>
        public virtual IUnitOfWork Create()
        {
            return CreateCore(null);
        }

        /// <summary>
        /// Creates a new unit of work instance with specified parameters.
        /// </summary>
        protected IUnitOfWork CreateCore(object parameter)
        {
            var uow = CreateUnitOfWork(parameter);
            registry.RegisterUnitOfWork(uow);
            uow.Disposing += OnUnitOfWorkDisposing;
            return uow;
        }

        /// <summary>
        /// Creates the real unit of work instance.
        /// </summary>
        protected abstract IUnitOfWork CreateUnitOfWork(object parameter);


        /// <summary>
        /// Called when the unit of work is being disposed.
        /// </summary>
        private void OnUnitOfWorkDisposing(object sender, EventArgs e)
        {
            registry.UnregisterUnitOfWork((IUnitOfWork) sender);
        }


        /// <summary>
        /// Gets the unit of work in the current scope.
        /// </summary>
        public IUnitOfWork GetCurrent(int ancestorLevel = 0)
        {
            return registry.GetCurrent(ancestorLevel);
        }
    }
}