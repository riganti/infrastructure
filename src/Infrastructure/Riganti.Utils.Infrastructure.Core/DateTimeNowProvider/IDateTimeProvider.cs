using System;

namespace Riganti.Utils.Infrastructure.Core
{
    /// <summary>
    /// Custom provide of current time.
    /// </summary>
    public interface IDateTimeProvider
    {
        /// <summary>
        /// Gets current time
        /// </summary>
        DateTime Now { get; }
    }
}