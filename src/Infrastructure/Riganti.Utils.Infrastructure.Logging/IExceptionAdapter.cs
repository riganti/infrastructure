using System;
using System.Collections.Generic;
using System.Text;

namespace Riganti.Utils.Infrastructure.Logging
{
    public interface IExceptionAdapter
    {
        void AppendFormatedDetails(Exception exception, StringBuilder sb);

        void ModifyIgnoredReflectionProperties(Exception exception, List<string> collection);
    }
}