using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;

namespace Riganti.Utils.Infrastructure.Azure.TableStorage.Tests.Entities
{
    public class Band : TableEntity
    {
        public string Id => RowKey;
        public string Title { get; set; }

        public ICollection<Musician> Employees { get; }

        public Band(Guid id, string title, string city) : base(city, id.ToString())
        {
            Employees = new List<Musician>();
        }
    }
}