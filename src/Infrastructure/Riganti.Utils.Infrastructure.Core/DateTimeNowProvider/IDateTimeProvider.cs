using System;

namespace Riganti.Utils.Infrastructure.Core
{
    /// <summary>
    /// Custom provider of current time.
    /// </summary>
    public interface IDateTimeProvider
    {
        /// <summary>
        /// Gets current time
        /// </summary>
        DateTime Now { get; }
    }
}