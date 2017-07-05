using Microsoft.EntityFrameworkCore;

// ReSharper disable once CheckNamespace
namespace Riganti.Utils.Infrastructure.EntityFramework.Tests.Repository
{
    public class YesMinisterDbContext : DbContext
    {
        public virtual DbSet<EpisodeEntity> Episodes { get; set; }
        public virtual DbSet<QuoteEntity> Quotes { get; set; }
    }
}