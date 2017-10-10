using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Riganti.Utils.Infrastructure.Core;
using Riganti.Utils.Infrastructure.Services.Facades;

namespace Riganti.Utils.Infrastructure.Services.Tests.Facades
{
    public class EmployeeProjectsFacade : TemporalRelationshipCrudFacade<EmployeeProject, Employee, int, EmployeeProjectDTO, EmployeeProjectFilterDTO>
    {
        public override Func<EmployeeProject, int> ParentEntityKeySelector => ep => ep.EmployeeId;
        public override Func<EmployeeProjectDTO, int> ParentDTOKeySelector => ep => ep.EmployeeId;
        public override Func<EmployeeProject, int> ChildEntityKeySelector => ep => ep.ProjectId;
        public override Func<EmployeeProjectDTO, int> ChildDTOKeySelector => ep => ep.ProjectId;
        public override Expression<Func<Employee, ICollection<EmployeeProject>>> ChildEntityCollectionSelector => e => e.EmployeeProjects;
        public override Expression<Func<Employee, object>> ChildEntityCollectionExpression => e => e.EmployeeProjects;
        public override Expression<Func<Employee, object>>[] AdditionalParentIncludes => new Expression<Func<Employee, object>>[0];

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