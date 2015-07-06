using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using LinqToDB;

using NuClear.Messaging.API.Flows.Metadata;
using NuClear.Messaging.API.Receivers;
using NuClear.Model.Common.Entities;
using NuClear.OperationsProcessing.API.Final;
using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.Telemetry;

namespace NuClear.Replication.OperationsProcessing.Transports.SQLStore
{
    public sealed class SqlStoreReceiver : MessageReceiverBase<PerformedOperationsFinalProcessingMessage, IFinalProcessingQueueReceiverSettings> 
    {
        private readonly IDataContext _context;
        private readonly ITelemetryPublisher _telemetryPublisher;

        public SqlStoreReceiver(MessageFlowMetadata sourceFlowMetadata, IFinalProcessingQueueReceiverSettings messageReceiverSettings, IDataContext context, ITelemetryPublisher telemetryPublisher)
            : base(sourceFlowMetadata, messageReceiverSettings)
        {
            _context = context;
            _telemetryPublisher = telemetryPublisher;
        }

        protected override IReadOnlyList<PerformedOperationsFinalProcessingMessage> Peek()
        {
            IReadOnlyList<PerformedOperationsFinalProcessingMessage> messages;

            using (var scope = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                messages = _context.GetTable<PerformedOperationFinalProcessing>()
                                      .Where(processing => processing.MessageFlowId == SourceFlowMetadata.MessageFlow.Id)
                                      .Take(MessageReceiverSettings.BatchSize)
                                      .AsEnumerable()
                                      // fake grouping
                                      .GroupBy(x => 0)
                                      .Select(x => new PerformedOperationsFinalProcessingMessage
                                        {
                                            EntityId = 0,
                                            MaxAttemptCount = 0,
                                            EntityType = EntityType.Instance.None(),
                                            Flow = SourceFlowMetadata.MessageFlow,
                                            FinalProcessings = x,
                                        }).ToList();

                scope.Complete();
            }

            _telemetryPublisher.Trace("Peek", new { MessageCount = messages.Count });
            return messages;
        }

        protected override void Complete(IEnumerable<PerformedOperationsFinalProcessingMessage> successfullyProcessedMessages, IEnumerable<PerformedOperationsFinalProcessingMessage> failedProcessedMessages)
        {
            // COMMENT {all, 05.05.2015}: Что делать при ошибках во время обработки?
            // Сейчас и на стадии Primary и на стадии Final сообщение будет пытаться обработаться до тех пор, пока не получится.
            // Или пока админ не удалит его из очереди.
            _telemetryPublisher.Trace("Complete", new { SuccessCount = successfullyProcessedMessages.Count(), FailCount = failedProcessedMessages.Count() });

            using (var scope = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                foreach (var message in successfullyProcessedMessages.SelectMany(message => message.FinalProcessings))
                {
                    _context.Delete(message);
                }

                scope.Complete();
            }
        }
    }
}
