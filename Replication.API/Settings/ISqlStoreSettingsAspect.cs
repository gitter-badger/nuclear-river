using NuClear.Settings.API;

namespace NuClear.AdvancedSearch.Replication.API.Settings
{
    public interface ISqlStoreSettingsAspect : ISettings
    {
        int SqlCommandTimeout { get; }
    }
}