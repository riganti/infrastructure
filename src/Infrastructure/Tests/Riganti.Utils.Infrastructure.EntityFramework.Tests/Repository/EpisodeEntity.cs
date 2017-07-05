using Riganti.Utils.Infrastructure.Core;

#if EFCORE
namespace Riganti.Utils.Infrastructure.EntityFrameworkCore.Tests.Repository
#else
namespace Riganti.Utils.Infrastructure.EntityFramework.Tests.Repository
#endif
{
    public class EpisodeEntity : IEntity<int>
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int Series { get; set; }
    }
}