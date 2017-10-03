using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Riganti.Utils.Infrastructure.Core;
using Riganti.Utils.Infrastructure.Services.Facades;

namespace Riganti.Utils.Infrastructure.Services.Tests.Facades
{
    public class EmployeeProjectsFacade : TemporalRelationshipCrudFacade<EmployeeProject, Employee, int, EmployeeProjectDTO, EmployeeProjectFilterDTO>
    {
        public override Func<EmployeeProject, int> ParentIdValueEntityFunc => ep => ep.EmployeeId;
        public override Func<EmployeeProjectDTO, int> ParentIdValueDtoFunc => ep => ep.EmployeeId;
        public override Func<EmployeeProject, int> IdentifierValueEntityFunc => ep => ep.ProjectId;
        public override Func<EmployeeProjectDTO, int> IdentifierValueDtoFunc => ep => ep.ProjectId;
        public override Func<Employee, ICollection<EmployeeProject>> EntityCollectionFunc => e => e.EmployeeProjects;
        public override Expression<Func<Employee, object>> Includes => e => e.EmployeeProjects;

        public EmployeeProjectsFacade(Func<IFilteredQuery<EmployeeProjectDTO, EmployeeProjectFilterDTO>> queryFactory,
                                      IEntityDTOMapper<EmployeeProject, EmployeeProjectDTO> entityMapper,
                                      IRepository<EmployeeProject, int> repository,
                                      IRepository<Employee, int> parentRepository,
                                      IDateTimeProvider dateTimeProvider,
                                      IUnitOfWorkProvider unitOfWorkProvider)
            : base(queryFactory, entityMapper, repository, parentRepository, dateTimeProvider, unitOfWorkProvider)
        {
        }
    }
}