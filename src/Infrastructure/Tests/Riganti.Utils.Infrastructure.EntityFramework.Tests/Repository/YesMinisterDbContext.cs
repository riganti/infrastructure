#if EFCORE
using Microsoft.EntityFrameworkCore;
#else
using System.Data.Entity;
#endif

#if EFCORE
namespace Riganti.Utils.Infrastructure.EntityFrameworkCore.Tests.Repository
#else
namespace Riganti.Utils.Infrastructure.EntityFramework.Tests.Repository
#endif
{
    public class YesMinisterDbContext : DbContext
    {
        public virtual DbSet<EpisodeEntity> Episodes { get; set; }
        public virtual DbSet<QuoteEntity> Quotes { get; set; }
    }
}