using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.EntityFramework.Tests.Repository
{
    public class EpisodeEntity : IEntity<int>
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int Series { get; set; }
    }
}