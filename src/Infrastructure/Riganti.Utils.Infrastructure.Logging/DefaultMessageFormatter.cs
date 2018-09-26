using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.Logging
{
    /// <summary>
    /// Default log formater with timestamp and severity.
    /// </summary>
    public class DefaultMessageFormatter : IMessageFormatter
    {
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly string timestampFormat;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMessageFormatter"/> class
        /// with default timestamp format "yyyy-MM-dd HH:mm:ss.fff".
        /// </summary>
        public DefaultMessageFormatter(IDateTimeProvider dateTimeProvider)
            : this(dateTimeProvider, "yyyy-MM-dd HH:mm:ss.fff")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMessageFormatter"/> class
        /// with <param name="timestampFormat"></param>.
        /// </summary>
        /// <param name="dateTimeProvider">Current time provider.</param>
        /// <param name="timestampFormat">The timestamp format.</param>
        public DefaultMessageFormatter(IDateTimeProvider dateTimeProvider, string timestampFormat)
        {
            this.dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
            this.timestampFormat = timestampFormat ?? throw new ArgumentNullException(nameof(timestampFormat));
        }

        /// <summary>
        /// Formats the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="additionalData">The additional data.</param>
        /// <param name="severity">The severity.</param>
        /// <param name="sb">StringBuilder.</param>
        public void Format(string message, IDictionary<string, string> additionalData, Severity severity, StringBuilder sb)
        {
            sb.Append($"{dateTimeProvider.Now.ToString(timestampFormat)} [{severity}] [{Thread.CurrentThread.ManagedThreadId}] {message}");

            if (additionalData != null && additionalData.Any())
                sb.Append($" {{{String.Join("|", additionalData.Select(e => $"{e.Key}={e.Value}"))}}}");
        }
    }
}