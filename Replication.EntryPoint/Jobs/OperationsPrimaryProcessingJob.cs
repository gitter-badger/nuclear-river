using System;

using NuClear.Jobs;
using NuClear.Messaging.API.Flows.Metadata;
using NuClear.Messaging.API.Processing.Processors;
using NuClear.Messaging.API.Processing.Stages;
using NuClear.Metamodeling.Provider;
using NuClear.OperationsProcessing.API;
using NuClear.OperationsProcessing.API.Metadata;
using NuClear.OperationsProcessing.API.Primary;
using NuClear.Security.API;
using NuClear.Telemetry;
using NuClear.Tracing.API;
using NuClear.Utils;

using Quartz;

namespace NuClear.AdvancedSearch.Replication.EntryPoint.Jobs
{
    [DisallowConcurrentExecution]
    public class OperationsPrimaryProcessingJob : TaskServiceJobBase, IInterruptableJob
    {
        private readonly object _sync = new object();
        private readonly IMetadataProvider _metadataProvider;
        private readonly IMessageFlowProcessorFactory _messageFlowProcessorFactory;
        private readonly ITelemetryPublisher _telemetryPublisher;

        private IAsyncMessageFlowProcessor _performedOperationsProcessor;

        public OperationsPrimaryProcessingJob(
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
        }

        public int BatchSize { get; set; }
        public string Flow { get; set; }
        public int? TimeSafetyOffsetHours { get; set; }

        public int? BaseDelay { get; set; }
        public int? DelayAfterFailure { get; set; }
        public int? DelayIncrement { get; set; }
        public int? MaxDelay { get; set; }
        public int? SufficientBatchUtilizationThreshold { get; set; }

        private IAsyncMessageFlowProcessor MessageFlowProcessor
        {
            get
            {
                lock (_sync)
                {
                    return _performedOperationsProcessor;
                }
            }
            set
            {
                lock (_sync)
                {
                    _performedOperationsProcessor = value;
                }
            }
        }

        public void Interrupt()
        {
            var flowProcessor = MessageFlowProcessor;
            if (flowProcessor != null)
            {
                flowProcessor.Stop();
            }
        }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            _telemetryPublisher.Trace("Start");
            if (string.IsNullOrEmpty(Flow))
            {
                string msg = string.Format("Required job arg {0} is not specified, check job config", StaticReflection.GetMemberName(() => Flow));
                Tracer.Fatal(msg);
                throw new InvalidOperationException(msg);
            }

            MessageFlowMetadata messageFlowMetadata;
            if (!_metadataProvider.TryGetMetadata(Flow.AsPrimaryProcessingFlowId(), out messageFlowMetadata))
            {
                string msg = "Unsupported flow specified for processing: " + Flow;
                Tracer.Fatal(msg);
                throw new InvalidOperationException(msg);
            }

            Tracer.Debug("Launching message flow processing. Target message flow metadata: " + messageFlowMetadata);

            try
            {
                var processorSettings = new PerformedOperationsPrimaryFlowProcessorSettings
                                        {
                                            MessageBatchSize = BatchSize,
                                            AppropriatedStages = new[]
                                                                 {
                                                                     MessageProcessingStage.Transformation,
                                                                     MessageProcessingStage.Accumulation,
                                                                     MessageProcessingStage.Handling
                                                                 },
                                            FirstFaultTolerantStage = MessageProcessingStage.None,
                                            TimeSafetyOffsetHours = TimeSafetyOffsetHours,
                                        };

                MessageFlowProcessor = _messageFlowProcessorFactory.CreateAsync<IPerformedOperationsFlowProcessorSettings>(messageFlowMetadata, processorSettings);
            }
            catch (Exception ex)
            {
                Tracer.Error(ex, "Can't create processor for  specified flow " + messageFlowMetadata);
                throw;
            }

            var settings = new ThrottlingSettings
                           {
                               BaseDelay = BaseDelay ?? ThrottlingSettings.Default.BaseDelay,
                               DelayAfterFailure = DelayAfterFailure ?? ThrottlingSettings.Default.DelayAfterFailure,
                               DelayIncrement = DelayIncrement ?? ThrottlingSettings.Default.DelayIncrement,
                               MaxDelay = MaxDelay ?? ThrottlingSettings.Default.MaxDelay,
                               SufficientBatchUtilizationThreshold = SufficientBatchUtilizationThreshold ?? ThrottlingSettings.Default.SufficientBatchUtilizationThreshold,
                           };

            _telemetryPublisher.Trace("ThrottlingSettings", settings);

            try
            {
                Tracer.Debug("Message flow processor starting. Target message flow: " + messageFlowMetadata);
                MessageFlowProcessor.Start(settings);

                Tracer.Debug("Message flow processor started, waiting for finish ... Target message flow: " + messageFlowMetadata);
                MessageFlowProcessor.Wait();
                Tracer.Debug("Message flow processor finished. Target message flow: " + messageFlowMetadata);
            }
            catch (Exception ex)
            {
                Tracer.Fatal(ex, "Message flow processor unexpectedly interrupted. Target message flow: " + messageFlowMetadata);
                _telemetryPublisher.Trace("FatalException", ex);
                throw;
            }
            finally
            {
                _telemetryPublisher.Trace("Finalizing");
                var flowProcessor = MessageFlowProcessor;
                if (flowProcessor != null)
                {
                    flowProcessor.Dispose();
                }
            }
        }
    }
}