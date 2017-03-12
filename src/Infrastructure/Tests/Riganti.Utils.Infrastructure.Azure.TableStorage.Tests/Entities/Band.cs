using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;

namespace Riganti.Utils.Infrastructure.Azure.TableStorage.Tests.Entities
{
    public class Band : TableEntity
    {
        public string Title => PartitionKey;
        public string Id => RowKey;

        public ICollection<Musician> Employees { get; }

        public Band(string id, string title) : base(title, id)
        {
            Employees = new List<Musician>();
        }
    }
}