using System;

namespace Riganti.Utils.Infrastructure.Core
{
    /// <summary>
    /// Represents an entity which is deleted by setting the DeletedDate property to current date.
    /// </summary>
    public interface ISoftDeleteEntity
    {

        DateTime? DeletedDate { get; set; }

    }
}
