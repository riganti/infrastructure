using System;
using Riganti.Utils.Infrastructure.Core;

#if EFCORE
namespace Riganti.Utils.Infrastructure.EntityFrameworkCore.Tests.Repository
#else
namespace Riganti.Utils.Infrastructure.EntityFramework.Tests.Repository
#endif
{
    public class QuoteEntity : IEntity<int>, ISoftDeleteEntity
    {
        public int Id { get; set; }
        public DateTime? DeletedDate { get; set; }

        public string Text { get; set; }
    }
}