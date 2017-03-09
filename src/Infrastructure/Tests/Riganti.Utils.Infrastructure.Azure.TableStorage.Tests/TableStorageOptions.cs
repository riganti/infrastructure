using System;
using System.Collections.Generic;
using System.Text;

namespace Riganti.Utils.Infrastructure.Azure.TableStorage.Tests
{
    public class CreateOwnContextTableStorageOptions : ITableStorageOptions
    {
        public string StorageConnectionString => "UseDevelopmentStorage=true";
        public TableStorageContextOptions ContextOptions => TableStorageContextOptions.AlwaysCreateOwnContext;
    }

    public class ReuseParentContextTableStorageOptions : ITableStorageOptions
    {
        public string StorageConnectionString => "UseDevelopmentStorage=true";
        public TableStorageContextOptions ContextOptions => TableStorageContextOptions.ReuseParentContext;
    }
}
