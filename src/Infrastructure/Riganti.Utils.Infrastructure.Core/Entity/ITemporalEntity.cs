using System;

namespace Riganti.Utils.Infrastructure.Core
{
    /// <summary>
    /// Represents an entity with time-restricted validity.
    /// </summary>
    public interface ITemporalEntity
    {
        /// <summary>
        /// Gets or sets the date (inclusive) since which this entity is valid
        /// or null for entities with unrestricted validity.
        /// </summary>
        DateTime? ValidityBeginDate { get; set; }

        /// <summary>
        /// Gets or sets the date (exclusive) til which this entity is valid
        /// or null for entities with unrestricted validity.
        /// </summary>
        DateTime? ValidityEndDate { get; set; }
    }
}