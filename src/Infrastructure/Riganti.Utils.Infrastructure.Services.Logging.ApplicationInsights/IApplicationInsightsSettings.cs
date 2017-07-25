// ReSharper disable once CheckNamespace
namespace Riganti.Utils.Infrastructure.Services.Logging
{
    public interface IApplicationInsightsSettings
    {
        string InstrumentationKey { get; }
    }
}