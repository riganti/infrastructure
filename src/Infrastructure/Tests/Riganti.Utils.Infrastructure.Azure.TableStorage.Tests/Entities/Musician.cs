using System;
using System.Text;
using Microsoft.WindowsAzure.Storage.Table;

namespace Riganti.Utils.Infrastructure.Azure.TableStorage.Tests.Entities
{
    public class Musician : TableEntity
    {
        public string Name { get; set; }
        public string Band => PartitionKey;
        public string Email => RowKey;

        public Musician() { }

        public Musician(string name, string band, string email) 
            : base(band, email)
        {
            Name = name;
        }
    }
}
