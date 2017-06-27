using System;

namespace Riganti.Utils.Infrastructure.Azure.TableStorage.TableEntityMappers
{
    public class TypeNameTableEntityMapper : TableEntityMapperBase
    {
        public override string GetTable(Type type)
        {
            return type.Name;
        }
    }
}
