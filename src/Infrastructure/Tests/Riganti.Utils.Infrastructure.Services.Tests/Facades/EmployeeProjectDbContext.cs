using Microsoft.EntityFrameworkCore;

namespace Riganti.Utils.Infrastructure.Services.Tests.Facades
{
    public class EmployeeProjectDbContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<EmployeeProject> EmployeeProjects { get; set; }

        public EmployeeProjectDbContext()
        {
        }

        public EmployeeProjectDbContext(DbContextOptions<EmployeeProjectDbContext> options)
            : base(options)
        {
        }
    }
}
