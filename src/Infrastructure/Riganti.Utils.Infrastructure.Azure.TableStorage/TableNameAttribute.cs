using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riganti.Utils.Infrastructure.Azure.TableStorage
{
    /// <summary>
    /// Decoration for table name mapping
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class TableNameAttribute : Attribute
    {
        public readonly string Name;

        public TableNameAttribute(string name)
        {
            this.Name = name;
        }

    }
}
