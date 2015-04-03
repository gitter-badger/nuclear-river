using NuClear.Settings.API;

namespace Replication.EntryPoint.Settings
{
    public interface IEnvironmentSettings : ISettings
    {
        string EnvironmentName { get; }
        string EntryPointName { get; }
    }
}