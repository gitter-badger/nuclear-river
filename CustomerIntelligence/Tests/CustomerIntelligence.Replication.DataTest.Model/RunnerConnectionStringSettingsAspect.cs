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

        private static ConnectionStringsSection GetTestAssemblyConnectionStrings()
        {
            var assemblyLocation = typeof(RunnerConnectionStringSettingsAspect).Assembly.Location;
            return ConfigurationManager.OpenExeConfiguration(assemblyLocation).ConnectionStrings;
        }

        private static IReadOnlyDictionary<IConnectionStringIdentity, string> CreateConnectionStringMappings(ConnectionStringsSection configuration) =>
            new Dictionary<IConnectionStringIdentity, string>
            {
                { ErmConnectionStringIdentity.Instance, configuration.ConnectionStrings["Erm"].ConnectionString },
                { FactsConnectionStringIdentity.Instance, configuration.ConnectionStrings["Facts"].ConnectionString },
                { CustomerIntelligenceConnectionStringIdentity.Instance, configuration.ConnectionStrings["CustomerIntelligence"].ConnectionString },
                { BitConnectionStringIdentity.Instance, configuration.ConnectionStrings["Bit"].ConnectionString },
                { StatisticsConnectionStringIdentity.Instance, configuration.ConnectionStrings["Statistics"].ConnectionString },

                { ErmMasterConnectionStringIdentity.Instance, configuration.ConnectionStrings["ErmMaster"].ConnectionString },
            };
    }
}