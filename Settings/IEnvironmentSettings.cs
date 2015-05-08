using NuClear.Settings.API;

namespace NuClear.AdvancedSearch.Settings
{
    public interface IEnvironmentSettings : ISettings
    {
        string EnvironmentName { get; }
        string EntryPointName { get; }
    }
}
