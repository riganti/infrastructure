namespace Riganti.Utils.Infrastructure.Services.Facades
{
    public interface IEntityDTOMapper<TEntity, TDTO>
    {
        TDTO MapToDTO(TEntity source);

        TEntity MapToEntity(TDTO source);

        void PopulateEntity(TDTO source, TEntity target);
    }
}