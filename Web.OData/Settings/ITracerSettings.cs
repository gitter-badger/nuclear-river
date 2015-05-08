using NuClear.Settings.API;

namespace NuClear.AdvancedSearch.Web.OData.Settings
{
    public interface ITracerSettings : ISettings
    {
        string EnvironmentName { get; }
        string EntryPointName { get; }
        string ConnectionString { get; }
    }
}