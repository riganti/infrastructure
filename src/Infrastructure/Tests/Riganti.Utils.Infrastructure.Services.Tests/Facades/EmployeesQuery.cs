using System.Linq;
using AutoMapper.QueryableExtensions;
using Riganti.Utils.Infrastructure.Core;
using Riganti.Utils.Infrastructure.EntityFrameworkCore;

namespace Riganti.Utils.Infrastructure.Services.Tests.Facades
{
    public class EmployeesQuery : EntityFrameworkQuery<EmployeeDTO, EmployeeProjectDbContext>
    {

        public EmployeesQuery(IUnitOfWorkProvider unitOfWorkProvider)
            : base(unitOfWorkProvider)
        {
        }

        protected override IQueryable<EmployeeDTO> GetQueryable()
        {
            return Context.Employees.ProjectTo<EmployeeDTO>();
        }
    }
}