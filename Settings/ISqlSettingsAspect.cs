using NuClear.Settings.API;

namespace NuClear.AdvancedSearch.Settings
{
    public interface ISqlSettingsAspect : ISettings
    {
        int SqlCommandTimeout { get; }
    }
}