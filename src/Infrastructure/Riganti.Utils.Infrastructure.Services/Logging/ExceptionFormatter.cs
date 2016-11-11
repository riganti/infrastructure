using System;

namespace Riganti.Utils.Infrastructure.Services.Logging
{
    public interface IExceptionFormatter
    {

        string FormatException(Exception ex);

    }
}
