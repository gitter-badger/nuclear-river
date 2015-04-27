using System.Configuration;

using NuClear.Settings.API;

namespace NuClear.AdvancedSearch.Replication.EntryPoint.Settings
{
    public interface IConnectionStringSettings : ISettings
    {
        string GetConnectionString(ConnectionStringName connectionStringNameAlias);
        ConnectionStringSettings GetConnectionStringSettings(ConnectionStringName connectionStringNameAlias);
    }
}
