using NuClear.Settings.API;

namespace NuClear.Replication.Core.API.Settings
{
    public interface ISqlStoreSettingsAspect : ISettings
    {
        int SqlCommandTimeout { get; }
    }
}