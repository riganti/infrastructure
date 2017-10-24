using System;
using Riganti.Utils.Infrastructure.Core;
using Riganti.Utils.Infrastructure.Services.Facades;

namespace Riganti.Utils.Infrastructure.Services.Tests.Facades
{
    public class EmployeesFacade : CrudFacadeBase<Employee, int, EmployeeDTO, EmployeeDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CrudFacadeBase{TEntity, TKey, TListDTO, TDetailDTO}"/> class.
        /// </summary>
        public EmployeesFacade(Func<IQuery<EmployeeDTO>> queryFactory,
                               IRepository<Employee, int> repository,
                               IEntityDTOMapper<Employee, EmployeeDTO> mapper,
                               IUnitOfWorkProvider unitOfWorkProvider)
            : base(queryFactory, repository, mapper)
        {
            UnitOfWorkProvider = unitOfWorkProvider;
        }
    }
}
