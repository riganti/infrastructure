using System;

namespace Riganti.Utils.Infrastructure.Azure.TableStorage
{
    public class CloudTableNotFoundException : Exception
    {
        private readonly string tableName;

        public CloudTableNotFoundException(string tableName) 
            : base($"This cloud table isn't found: {tableName}")
        {
            this.tableName = tableName;
        }
    }
}
