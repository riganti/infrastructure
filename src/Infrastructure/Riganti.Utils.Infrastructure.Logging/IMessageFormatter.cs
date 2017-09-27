using System.Collections.Generic;
using System.Text;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.Logging
{
    public interface IMessageFormatter
    {
        /// <summary>
        /// Formats the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="additionalData">Additional data.</param>
        /// <param name="severity">The severity.</param>
        /// <param name="stringBuilder"></param>
        void Format(string message, IDictionary<string, string> additionalData, Severity severity, StringBuilder stringBuilder);
    }
}