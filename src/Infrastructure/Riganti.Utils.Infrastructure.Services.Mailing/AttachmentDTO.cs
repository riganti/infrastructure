using System.IO;

namespace Riganti.Utils.Infrastructure.Services.Mailing
{
    public class AttachmentDTO
    {

        public Stream Stream { get; set; }

        public string Name { get; set; }

        public string MimeType { get; set; }

    }
}