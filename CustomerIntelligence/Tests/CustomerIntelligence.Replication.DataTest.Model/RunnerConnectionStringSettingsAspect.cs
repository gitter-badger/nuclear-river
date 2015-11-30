using System.Collections.Generic;
using System.Configuration;

using CustomerIntelligence.Replication.DataTest.Model.Identitites.Connections;

using NuClear.CustomerIntelligence.Storage.Identitites.Connections;
using NuClear.Storage.API.ConnectionStrings;

namespace CustomerIntelligence.Replication.DataTest.Model
{
    public sealed class RunnerConnectionStringSettingsAspect : ConnectionStringSettingsAspect
    {
        public RunnerConnectionStringSettingsAspect()
            : base(CreateConnectionStringMappings(GetTestAssemblyConnectionStrings()))
        {
        }

        private static ConnectionStringSettingsCollection GetTestAssemblyConnectionStrings()
        {
            var assemblyLocation = typeof(RunnerConnectionStringSettingsAspect).Assembly.Location;
            return ConfigurationManager.OpenExeConfiguration(assemblyLocation).ConnectionStrings.ConnectionStrings;
        }

        private static IReadOnlyDictionary<IConnectionStringIdentity, string> CreateConnectionStringMappings(ConnectionStringSettingsCollection configuration) =>
            new Dictionary<IConnectionStringIdentity, string>
            {
                { ErmConnectionStringIdentity.Instance, configuration[ConnectionStringName.Erm].ConnectionString },
                { FactsConnectionStringIdentity.Instance, configuration[ConnectionStringName.Facts].ConnectionString },
                { CustomerIntelligenceConnectionStringIdentity.Instance, configuration[ConnectionStringName.CustomerIntelligence].ConnectionString },
                { BitConnectionStringIdentity.Instance, configuration[ConnectionStringName.Bit].ConnectionString },
                { StatisticsConnectionStringIdentity.Instance, configuration[ConnectionStringName.Statistics].ConnectionString },

                { ErmMasterConnectionStringIdentity.Instance, configuration[ConnectionStringName.ErmMaster].ConnectionString },
            };
    }
}