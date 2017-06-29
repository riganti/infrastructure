using System.Data.Entity;

namespace Riganti.Utils.Infrastructure.EntityFramework.Tests.Repository
{
    public class YesMinisterDbContext : DbContext
    {
        public virtual IDbSet<EpisodeEntity> Episodes { get; set; }
        public virtual IDbSet<QuoteEntity> Quotes { get; set; }
    }
}