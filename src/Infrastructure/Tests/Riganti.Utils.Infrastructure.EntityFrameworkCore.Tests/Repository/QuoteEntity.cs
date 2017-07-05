using System;
using Riganti.Utils.Infrastructure.Core;

// ReSharper disable once CheckNamespace
namespace Riganti.Utils.Infrastructure.EntityFramework.Tests.Repository
{
  public class QuoteEntity : IEntity<int>, ISoftDeleteEntity
  {
    public int Id { get; set; }
    public DateTime? DeletedDate { get; set; }

    public string Text { get; set; }
  }
}