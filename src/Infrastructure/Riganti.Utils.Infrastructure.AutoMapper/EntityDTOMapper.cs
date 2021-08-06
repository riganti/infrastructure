using AutoMapper;
using Riganti.Utils.Infrastructure.Services.Facades;

namespace Riganti.Utils.Infrastructure.AutoMapper
{
    public class EntityDTOMapper<TEntity, TDTO> : IEntityDTOMapper<TEntity, TDTO>
    {
        public TDTO MapToDTO(TEntity source)
        {
            return Mapper.Map<TDTO>(source);
        }

        public TEntity MapToEntity(TDTO source)
        {
            return Mapper.Map<TEntity>(source);
        }

        public void PopulateEntity(TDTO source, TEntity target)
        {
            Mapper.Map(source, target);
        }
    }

}
