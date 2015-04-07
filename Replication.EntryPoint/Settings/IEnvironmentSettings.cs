using NuClear.Settings.API;

namespace NuClear.AdvancedSearch.Replication.EntryPoint.Settings
{
    public interface IEnvironmentSettings : ISettings
    {
        string EnvironmentName { get; }
        string EntryPointName { get; }
    }
}