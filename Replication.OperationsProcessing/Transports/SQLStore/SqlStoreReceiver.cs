using System.Collections.Generic;
using System.Data;
using System.Linq;

using LinqToDB;
using LinqToDB.Data;

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
        private readonly DataConnection _dataConnection;

        public SqlStoreReceiver(MessageFlowMetadata sourceFlowMetadata, IFinalProcessingQueueReceiverSettings messageReceiverSettings, IDataContext context)
            : base(sourceFlowMetadata, messageReceiverSettings)
        {
            _dataConnection = (DataConnection)context;
        }

        protected override IReadOnlyList<PerformedOperationsFinalProcessingMessage> Peek()
        {
            ICollection<PerformedOperationFinalProcessing> messages;

            try
            {
                _dataConnection.BeginTransaction(IsolationLevel.ReadCommitted);

                messages = _dataConnection.GetTable<PerformedOperationFinalProcessing>()
                                          .Where(processing => processing.MessageFlowId == SourceFlowMetadata.MessageFlow.Id)
                                          .Take(MessageReceiverSettings.BatchSize)
                                          .ToList();

                _dataConnection.CommitTransaction();
            }
            catch
            {
                _dataConnection.RollbackTransaction();
                throw;
            }

            return new[]
                   {
                       new PerformedOperationsFinalProcessingMessage
                       {
                           EntityId = 0,
                           MaxAttemptCount = 0,
                           EntityType = EntityType.Instance.None(),
                           Flow = SourceFlowMetadata.MessageFlow,
                           FinalProcessings = messages,
                       }
                   };
        }

        protected override void Complete(IEnumerable<PerformedOperationsFinalProcessingMessage> successfullyProcessedMessages, IEnumerable<PerformedOperationsFinalProcessingMessage> failedProcessedMessages)
        {
            // COMMENT {all, 05.05.2015}: Что делать при ошибках во время обработки?
            // Сейчас и на стадии Primary и на стадии Final сообщение будет пытаться обработаться до тех пор, пока не получится.
            // Или пока админ не удалит его из очереди.
            try
            {
                _dataConnection.BeginTransaction(IsolationLevel.ReadCommitted);

                foreach (var message in successfullyProcessedMessages.SelectMany(message => message.FinalProcessings))
                {
                    _dataConnection.Delete(message);
                }

                _dataConnection.CommitTransaction();
            }
            catch
            {
                _dataConnection.RollbackTransaction();
                throw;
            }
        }
    }
}
