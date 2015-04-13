using System;

using NuClear.Jobs;
using NuClear.Messaging.API.Flows.Metadata;
using NuClear.Messaging.API.Processing.Processors;
using NuClear.Messaging.API.Processing.Stages;
using NuClear.Metamodeling.Provider;
using NuClear.Metamodeling.Utils;
using NuClear.OperationsProcessing.API;
using NuClear.OperationsProcessing.API.Metadata;
using NuClear.OperationsProcessing.API.Primary;
using NuClear.Security.API;
using NuClear.Tracing.API;

using Quartz;

namespace NuClear.AdvancedSearch.Replication.EntryPoint.Jobs
{
    [DisallowConcurrentExecution]
    public class OperationsPrimaryProcessingJob : TaskServiceJobBase, IInterruptableJob
    {
        private readonly object _sync = new object();
        private readonly IMetadataProvider _metadataProvider;
        private readonly IMessageFlowProcessorFactory _messageFlowProcessorFactory;

        private IAsyncMessageFlowProcessor _performedOperationsProcessor;

        public OperationsPrimaryProcessingJob(
            IMetadataProvider metadataProvider,
            IMessageFlowProcessorFactory messageFlowProcessorFactory,
            ISignInService signInService,
            IUserImpersonationService userImpersonationService,
            ITracer tracer)
            : base(signInService, userImpersonationService, tracer)
        {
            _metadataProvider = metadataProvider;
            _messageFlowProcessorFactory = messageFlowProcessorFactory;
        }

        public int BatchSize { get; set; }
        public string Flow { get; set; }
        public int? TimeSafetyOffsetHours { get; set; }

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
                                                                     MessageProcessingStage.Transforming,
                                                                     MessageProcessingStage.Processing,
                                                                     MessageProcessingStage.Handle
                                                                 },
                                            FirstFaultTolerantStage = MessageProcessingStage.None,
                                            TimeSafetyOffsetHours = TimeSafetyOffsetHours
                                        };

                MessageFlowProcessor = _messageFlowProcessorFactory.CreateAsync<IPerformedOperationsFlowProcessorSettings>(messageFlowMetadata, processorSettings);
            }
            catch (Exception ex)
            {
                Tracer.Error(ex, "Can't create processor for  specified flow " + messageFlowMetadata);
                throw;
            }

            try
            {
                Tracer.Debug("Message flow processor starting. Target message flow: " + messageFlowMetadata);
                MessageFlowProcessor.Start();

                Tracer.Debug("Message flow processor started, waiting for finish ... Target message flow: " + messageFlowMetadata);
                MessageFlowProcessor.Wait();
                Tracer.Debug("Message flow processor finished. Target message flow: " + messageFlowMetadata);
            }
            catch (Exception ex)
            {
                Tracer.Fatal(ex, "Message flow processor unexpectedly interrupted. Target message flow: " + messageFlowMetadata);
                throw;
            }
            finally
            {
                var flowProcessor = MessageFlowProcessor;
                if (flowProcessor != null)
                {
                    flowProcessor.Dispose();
                }
            }
        }
    }
}