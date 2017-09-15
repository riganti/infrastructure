using System;
using System.Collections.Generic;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Riganti.Utils.Infrastructure.Core;

// ReSharper disable once CheckNamespace
namespace Riganti.Utils.Infrastructure.Logging
{
    public class ApplicationInsightsLogger : LoggerBase
    {
        private readonly TelemetryClient telemetry;

        public ApplicationInsightsLogger(IApplicationInsightsSettings settings, IDateTimeProvider dateTimeProvider, IEnumerable<IAdditionalDataProvider> additionalDataProviders = default(IEnumerable<IAdditionalDataProvider>)) : base(dateTimeProvider, additionalDataProviders)
        {
            Guard.ArgumentNotNull(settings, nameof(settings));

            telemetry = new TelemetryClient(new TelemetryConfiguration { InstrumentationKey = settings.InstrumentationKey });
        }

        protected override void LogMessageCore(string message, IDictionary<string, string> additionalData, Severity severity)
        {
            if (telemetry.IsEnabled())
            {
                telemetry.TrackTrace(new TraceTelemetry(message)
                {
                    Timestamp  = DateTimeProvider.Now,
                    SeverityLevel = MapSeverityLevel(severity)
                });
            }
        }
    

        protected override void LogExceptionCore(Exception exception, IDictionary<string, string> additionalData, Severity severity)
        {
            if (telemetry.IsEnabled())
            {
                telemetry.TrackException(new ExceptionTelemetry(exception)
                {
                    Timestamp = DateTimeProvider.Now,
                    SeverityLevel = MapSeverityLevel(severity)
                });
            }
        }

        private SeverityLevel MapSeverityLevel(Severity severity)
        {
            switch (severity)
            {
                case Severity.Critical: return SeverityLevel.Critical;
                case Severity.Error: return SeverityLevel.Error;
                case Severity.Warning: return SeverityLevel.Warning;
                case Severity.Verbose: return SeverityLevel.Verbose;
                default: return SeverityLevel.Information;
            }
        }
    }
}
