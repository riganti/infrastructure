using System;

namespace Riganti.Utils.Infrastructure.Logging
{
    public interface IExceptionFormatter
    {
        string FormatException(Exception ex);
    }
}
