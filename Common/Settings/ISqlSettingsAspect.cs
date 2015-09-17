using NuClear.Settings.API;

namespace NuClear.AdvancedSearch.Common.Settings
{
    public interface ISqlSettingsAspect : ISettings
    {
        int SqlCommandTimeout { get; }
    }
}