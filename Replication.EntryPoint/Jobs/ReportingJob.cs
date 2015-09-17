using System.Data;
using System.Data.SqlClient;

using Microsoft.ServiceBus;

using NuClear.AdvancedSearch.Common.Settings;
using NuClear.Jobs;
using NuClear.Replication.OperationsProcessing.Metadata.Flows;
using NuClear.Replication.OperationsProcessing.Performance;
using NuClear.Security.API;
using NuClear.Telemetry;
using NuClear.Telemetry.Probing;
using NuClear.Tracing.API;

using Quartz;

namespace NuClear.AdvancedSearch.Replication.EntryPoint.Jobs
{
    [DisallowConcurrentExecution]
    public sealed class ReportingJob : TaskServiceJobBase
    {
        private readonly ITelemetryPublisher _telemetry;
        private readonly NamespaceManager _manager;
        private readonly SqlConnection _sqlConnection;

        public ReportingJob(ITracer tracer,
                            ISignInService signInService,
                            IUserImpersonationService userImpersonationService,
                            ITelemetryPublisher telemetry,
                            IConnectionStringSettings connectionStringSettings)
            : base(signInService, userImpersonationService, tracer)
        {
            _telemetry = telemetry;
            _manager = NamespaceManager.CreateFromConnectionString(connectionStringSettings.GetConnectionString(ConnectionStringName.ServiceBus));
            _sqlConnection = new SqlConnection(connectionStringSettings.GetConnectionString(ConnectionStringName.CustomerIntelligence));
        }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            ReportPrimaryProcessingQueueLength();
            ReportFinalProcessingQueueLength();
            ReportProbes();
        }

        private void ReportProbes()
        {
            var reports = DefaultReportSink.Instance.ConsumeReports();
            foreach (var report in reports)
            {
                _telemetry.Trace("ProbeReport", report);
            }
        }

        private void ReportFinalProcessingQueueLength()
        {
            if (_sqlConnection.State != ConnectionState.Open)
            {
                _sqlConnection.Open();
            }

            const string CommandText = "select count(*) from Transport.PerformedOperationFinalProcessing " +
                                       "where MessageFlowId = @flowId";
            var command = new SqlCommand(CommandText, _sqlConnection);
            command.Parameters.Add("@flowId", SqlDbType.UniqueIdentifier);

            command.Parameters["@flowId"].Value = AggregatesFlow.Instance.Id;
            _telemetry.Publish<FinalProcessingAggregateQueueLengthIdentity>((int)command.ExecuteScalar());

            command.Parameters["@flowId"].Value = StatisticsFlow.Instance.Id;
            _telemetry.Publish<FinalProcessingStatisticsQueueLengthIdentity>((int)command.ExecuteScalar());

            _sqlConnection.Close();
        }

        private void ReportPrimaryProcessingQueueLength()
        {
            var subscription = _manager.GetSubscription("topic.advancedsearch", "9F2C5A2A-924C-485A-9790-9066631DB307");
            _telemetry.Publish<PrimaryProcessingQueueLengthIdentity>(subscription.MessageCountDetails.ActiveMessageCount);
        }
    }
}