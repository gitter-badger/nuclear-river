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
            : base(CreateConnectionStringMappings(GetTestAssemblyConfiguration()))
        {
        }

        private static Configuration GetTestAssemblyConfiguration()
        {
            var assemblyLocation = typeof(RunnerConnectionStringSettingsAspect).Assembly.Location;
            return ConfigurationManager.OpenExeConfiguration(assemblyLocation);
        }

        private static IReadOnlyDictionary<IConnectionStringIdentity, string> CreateConnectionStringMappings(Configuration configuration) =>
            new Dictionary<IConnectionStringIdentity, string>
            {
                { ErmConnectionStringIdentity.Instance, configuration.ConnectionStrings.ConnectionStrings["Erm"].ConnectionString },
                { FactsConnectionStringIdentity.Instance, configuration.ConnectionStrings.ConnectionStrings["Facts"].ConnectionString },
                { CustomerIntelligenceConnectionStringIdentity.Instance, configuration.ConnectionStrings.ConnectionStrings["CustomerIntelligence"].ConnectionString },
                { BitConnectionStringIdentity.Instance, configuration.ConnectionStrings.ConnectionStrings["Bit"].ConnectionString },
                { StatisticsConnectionStringIdentity.Instance, configuration.ConnectionStrings.ConnectionStrings["Statistics"].ConnectionString },

                { ErmMasterConnectionStringIdentity.Instance, configuration.ConnectionStrings.ConnectionStrings["ErmMaster"].ConnectionString },
            };
    }
}