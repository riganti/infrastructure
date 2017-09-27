namespace Riganti.Utils.Infrastructure.Core
{
    /// <summary>
    /// An interface for unit of work provider which is responsible for creating and managing unit of work objects.
    /// </summary>
    public interface IUnitOfWorkProvider
    {

        /// <summary>
        /// Creates a new unit of work.
        /// </summary>
        IUnitOfWork Create();

        /// <summary>
        /// Gets the unit of work in the current scope.
        /// </summary>
        /// <param name="ancestorLevel">0 means current unit of work, 1 means parent unit of work and so on.</param>
        IUnitOfWork GetCurrent(int ancestorLevel = 0);

    }
}