using AutoMapper;
using Riganti.Utils.Infrastructure.Services.Facades;

namespace Riganti.Utils.Infrastructure.AutoMapper
{
    public class EntityDTOMapper<TEntity, TDTO> : IEntityDTOMapper<TEntity, TDTO>
    {
        private readonly IMapper mapper;

        public EntityDTOMapper(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public TDTO MapToDTO(TEntity source)
        {
            return mapper.Map<TDTO>(source);
        }

        public TEntity MapToEntity(TDTO source)
        {
            return mapper.Map<TEntity>(source);
        }

        public void PopulateEntity(TDTO source, TEntity target)
        {
            mapper.Map(source, target);
        }
    }

}
