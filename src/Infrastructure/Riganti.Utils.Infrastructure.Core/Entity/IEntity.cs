namespace Riganti.Utils.Infrastructure.Core
{

    /// <summary>
    /// Represents an entity with single-column unique ID.
    /// </summary>
    public interface IEntity<TKey>
    {

        /// <summary>
        /// Gets or sets the unique identification of the entity.
        /// </summary>
        TKey Id { get; set; }

    }
}
