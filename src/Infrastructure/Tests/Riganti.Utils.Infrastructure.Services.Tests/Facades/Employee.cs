using System.Collections.Generic;
using Riganti.Utils.Infrastructure.Core;
using Riganti.Utils.Infrastructure.Services.Facades;

namespace Riganti.Utils.Infrastructure.Services.Tests.Facades
{
    public class Employee : IEntity<int>
    {
        public int Id { get; set; }
        public ICollection<EmployeeProject> EmployeeProjects { get; set; }
    }
}