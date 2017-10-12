using System.Collections.Generic;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.Services.Tests.Facades
{
    public class Project : IEntity<int>
    {
        public int Id { get; set; }
        public ICollection<EmployeeProject> EmployeeProjects { get; set; }
    }
}