using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riganti.Utils.Infrastructure.Services
{
    public interface ICurrentUserProvider<out TKey>
    {

        TKey Id { get; }

        string UserName { get; }

        string DisplayName { get; }

        string Email { get; }

        bool IsInRole(string roleName);
    }
}
