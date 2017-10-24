using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.Services.Tests.Facades
{
    public class EmployeeDTO : IEntity<int>
    {
        public int Id { get; set; }
    }
}