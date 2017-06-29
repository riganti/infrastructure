using System;

namespace Riganti.Utils.Infrastructure.Azure.TableStorage
{
    public class TypeNameTableEntityMapper : TableEntityMapperBase
    {
        public override string GetTable(Type type)
        {
            return type.Name;
        }
    }
}
