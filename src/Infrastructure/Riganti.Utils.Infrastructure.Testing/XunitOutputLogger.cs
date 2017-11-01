using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Riganti.Utils.Infrastructure.Core;
using Xunit.Abstractions;

namespace Riganti.Utils.Infrastructure.Testing
{
    public class XUnitOutputLogger : ILogger
    {
        private readonly ITestOutputHelper output;

        public XUnitOutputLogger(ITestOutputHelper output)
        {
            this.output = output;
        }

        public void LogCritical(string message)
        {
            output.WriteLine($"{GetTime()} {GetThreadId()} Critical: {message}");
        }

        public void LogError(string message)
        {
            output.WriteLine($"{GetTime()} {GetThreadId()} Error: {message}");
        }

        public void LogInfo(string message)
        {
            output.WriteLine($"{GetTime()} {GetThreadId()} Info: {message}");
        }

        public void LogVerbose(string message)
        {
            output.WriteLine($"{GetTime()} {GetThreadId()} Verbose: {message}");
        }

        public void LogWarning(string message)
        {
            output.WriteLine($"{GetTime()} {GetThreadId()} Warning: {message}");
        }

        public void LogException(Exception exception, IDictionary<string, string> additionalData = null, Severity severity = Severity.Error)
        {
            var addMsg = additionalData != null ? String.Join(" | ", additionalData.Select(x => $"{x.Key}={x.Value}")) : "";
            output.WriteLine($"{GetTime()} {GetThreadId()} {severity}: {exception}, Additional data: {addMsg}");
        }

        public void LogMessage(string message, IDictionary<string, string> additionalData = null, Severity severity = Severity.Info)
        {
            output.WriteLine($"{GetTime()} {GetThreadId()} {severity}: {message}");
        }

        private static string GetThreadId()
        {
            return $"[{Thread.CurrentThread.ManagedThreadId}]";
        }

        private static string GetTime()
        {
            return $"[{DateTime.Now:HH:mm:ss.fff}]";
        }
    }
}
