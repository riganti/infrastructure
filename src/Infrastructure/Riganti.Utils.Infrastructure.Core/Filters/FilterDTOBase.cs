namespace Riganti.Utils.Infrastructure.Core
{
    /// <summary>
    /// The base class for filters and filter groups.
    /// </summary>
    public abstract class FilterDTOBase
    {
        /// <summary>
        /// Returns whether the filter is valid and can be used.
        /// </summary>
        public abstract bool IsValid();
    }
}