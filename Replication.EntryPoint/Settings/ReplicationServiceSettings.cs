using System.Collections.Generic;
using System.Configuration;

using NuClear.AdvancedSearch.Common.Settings;
using NuClear.AdvancedSearch.Replication.API.Identitites.Connections;
using NuClear.IdentityService.Client.Settings;
using NuClear.OperationsLogging.Transports.ServiceBus;
using NuClear.Settings.API;
using NuClear.Storage.ConnectionStrings;
using NuClear.Telemetry.Logstash;

namespace NuClear.AdvancedSearch.Replication.EntryPoint.Settings
{
    public sealed class ReplicationServiceSettings : SettingsContainerBase
    {
        public ReplicationServiceSettings()
        {
            var connectionStringSettings = new ConnectionStringsSettingsAspect();

            Aspects.Use<ServiceBusMessageLockRenewalSettings>()
                   .Use<SqlSettingsAspect>()
                   .Use<EnvironmentSettingsAspect>()
                   .Use(new PersistentStoreAspect(connectionStringSettings))
                   .Use(new ServiceBusReceiverSettingsAspect(connectionStringSettings.GetConnectionString(ConnectionStringName.ServiceBus)))
                   .Use<TaskServiceProcessingSettingsAspect>()
                   .Use<CorporateBusSettingsAspect>()
                   .Use<LogstashSettingsAspect>()
                   .Use(connectionStringSettings)
                   .Use(new ConnectionStringSettingsAspect(new Dictionary<IConnectionStringIdentity, string>
                                                           {
                                                               {
                                                                   ErmConnectionStringIdentity.Instance,
                                                                   ConfigurationManager.ConnectionStrings["Erm"].ConnectionString
                                                               },
                                                               {
                                                                   FactsConnectionStringIdentity.Instance,
                                                                   ConfigurationManager.ConnectionStrings["Facts"].ConnectionString
                                                               },
                                                               {
                                                                   CustomerIntelligenceConnectionStringIdentity.Instance,
                                                                   ConfigurationManager.ConnectionStrings["CustomerIntelligence"].ConnectionString
                                                               },
                                                               {
                                                                   TransportConnectionStringIdentity.Instance,
                                                                   ConfigurationManager.ConnectionStrings["Transport"].ConnectionString
                                                               }
                                                           }))
                   .Use<ReplicationSettingsAspect>()
                   .Use<IdentityServiceClientSettingsAspect>();
        }
    }
}