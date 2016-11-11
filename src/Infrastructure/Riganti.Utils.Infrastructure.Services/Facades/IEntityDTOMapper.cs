namespace Riganti.Utils.Infrastructure.Services.Facades
{
    public interface IEntityDTOMapper<in TEntity, TDTO>
    {

        TDTO MapToDTO(TEntity source);

        void PopulateEntity(TDTO source, TEntity target);

    }
}