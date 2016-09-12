using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riganti.Utils.Infrastructure.Services.Storage
{
    public interface IStorageFolder
    {

        Task<bool> FileExists(string name);
        
        Task SaveFile(string name, Stream stream);

        Task<Stream> LoadFile(string name);

    }
}
