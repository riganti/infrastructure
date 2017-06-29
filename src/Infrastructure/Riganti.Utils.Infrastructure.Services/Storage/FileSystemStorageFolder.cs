using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Riganti.Utils.Infrastructure.Services.Storage
{
    public class FileSystemStorageFolder : IStorageFolder
    {
        private readonly string directory;

        public FileSystemStorageFolder(string directory)
        {
            this.directory = directory;

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        private string GetFullPath(string name)
        {
            if (name.Intersect(Path.GetInvalidFileNameChars()).Any())
            {
                throw new ArgumentException("The file name contains illegal characters!", nameof(name));
            }

            return Path.Combine(directory, name);
        }

        public Task<bool> FileExists(string name)
        {
            var fullPath = GetFullPath(name);
            return Task.FromResult(File.Exists(fullPath));
        }
        
        public async Task SaveFile(string name, Stream stream)
        {
            var fullPath = GetFullPath(name);
            using (var fs = new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                await stream.CopyToAsync(fs).ConfigureAwait(false);
                await stream.FlushAsync().ConfigureAwait(false);
                await fs.FlushAsync().ConfigureAwait(false);
            }
        }

        public Task<Stream> LoadFile(string name)
        {
            var fullPath = GetFullPath(name);

            Stream result = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            return Task.FromResult(result);
        }
    }
}