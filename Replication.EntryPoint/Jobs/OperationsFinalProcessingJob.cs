using System;

using NuClear.Jobs;
using NuClear.Messaging.API.Flows.Metadata;
using NuClear.Messaging.API.Processing.Processors;
using NuClear.Messaging.API.Processing.Stages;
using NuClear.Metamodeling.Provider;
using NuClear.OperationsProcessing.API.Final;
using NuClear.OperationsProcessing.API.Metadata;
using NuClear.Security.API;
using NuClear.Telemetry;
using NuClear.Telemetry.Probing;
using NuClear.Tracing.API;
using NuClear.Utils;

using Quartz;

namespace NuClear.AdvancedSearch.Replication.EntryPoint.Jobs
{
    [DisallowConcurrentExecution]
    public class OperationsFinalProcessingJob : TaskServiceJobBase
    {
        private readonly IMetadataProvider _metadataProvider;
        private readonly IMessageFlowProcessorFactory _messageFlowProcessorFactory;
        private readonly Lazy<MessageProcessingStage> _firstFaultTolerantStageSetting;
        private readonly ITelemetryPublisher _telemetryPublisher;

        public OperationsFinalProcessingJob(
            IMetadataProvider metadataProvider,
            IMessageFlowProcessorFactory messageFlowProcessorFactory,
            ISignInService signInService,
            IUserImpersonationService userImpersonationService,
            ITracer tracer, 
            ITelemetryPublisher telemetryPublisher)
            : base(signInService, userImpersonationService, tracer)
        {
            _metadataProvider = metadataProvider;
            _messageFlowProcessorFactory = messageFlowProcessorFactory;
            _telemetryPublisher = telemetryPublisher;
            _firstFaultTolerantStageSetting = new Lazy<MessageProcessingStage>(EvaluateFirstFaultTolerantStage);
        }

        public int BatchSize { get; set; }
        public string Flows { get; set; }
        public int? ReprocessingBatchSize { get; set; }
        public string FirstFaultTolerantStage { get; set; }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            if (string.IsNullOrEmpty(Flows))
            {
                string msg = string.Format("Required job arg {0} is not specified, check job config", StaticReflection.GetMemberName(() => Flows));
                Tracer.Fatal(msg);
                throw new InvalidOperationException(msg);
            }

            using (var probe = new Probe("ETL2 Job"))
            {
                var targetFlows = Flows.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var targetFlow in targetFlows)
                {
                    ProcessFlow(targetFlow);
                }
            }
        }

        private void ProcessFlow(string flow)
        {
            MessageFlowMetadata messageFlowMetadata;
            if (!_metadataProvider.TryGetMetadata(flow.AsFinalProcessingFlowId(), out messageFlowMetadata))
            {
                string msg = "Unsupported flow specified for processing: " + flow;
                Tracer.Fatal(msg);
                throw new InvalidOperationException(msg);
            }

            ISyncMessageFlowProcessor messageFlowProcessor;

            Tracer.Debug("Launching message flow processing. Target message flow: " + messageFlowMetadata);

            try
            {
                messageFlowProcessor = _messageFlowProcessorFactory.CreateSync<IPerformedOperationsFinalFlowProcessorSettings>(
                    messageFlowMetadata,
                    new PerformedOperationsFinalFlowProcessorSettings
                    {
                        MessageBatchSize = BatchSize,
                        AppropriatedStages = new[]
                                             {
                                                 MessageProcessingStage.Transformation,
                                                 MessageProcessingStage.Accumulation,
                                                 MessageProcessingStage.Handling
                                             },
                        FirstFaultTolerantStage = _firstFaultTolerantStageSetting.Value,
                        ReprocessingBatchSize = ReprocessingBatchSize ?? BatchSize
                    });
            }
            catch (Exception ex)
            {
                Tracer.Error(ex, "Can't create processor for specified flow " + messageFlowMetadata);
                throw;
            }

            try
            {
                Tracer.Debug("Message flow processor starting. Target message flow: " + messageFlowMetadata);
                messageFlowProcessor.Process();
                Tracer.Debug("Message flow processor finished. Target message flow: " + messageFlowMetadata);
            }
            catch (Exception ex)
            {
                _telemetryPublisher.Trace("Failure", ex);
                Tracer.Fatal(ex, "Message flow processor unexpectedly interrupted. Target message flow: " + messageFlowMetadata);
                throw;
            }
            finally
            {
                if (messageFlowProcessor != null)
                {
                    messageFlowProcessor.Dispose();
                }
            }
        }

        private MessageProcessingStage EvaluateFirstFaultTolerantStage()
        {
            if (string.IsNullOrEmpty(FirstFaultTolerantStage))
            {
                return MessageProcessingStage.None;
            }

            MessageProcessingStage stage;
            if (!Enum.TryParse(FirstFaultTolerantStage, true, out stage))
            {
                var msg = string.Format("FirstFaultTolerantStage setting for job with type {0} has invalid value: \"{1}\"" +
                                        "Please check settings",
                                        typeof(OperationsFinalProcessingJob),
                                        FirstFaultTolerantStage);
                Tracer.Error(msg);
                throw new InvalidOperationException(msg);
            }

            return stage;
        }
    }
}