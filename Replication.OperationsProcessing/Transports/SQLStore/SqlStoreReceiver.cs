using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using LinqToDB;

using NuClear.Messaging.API;
using NuClear.Messaging.API.Receivers;
using NuClear.Model.Common.Entities;
using NuClear.OperationsProcessing.Transports.SQLStore.Final;

namespace NuClear.Replication.OperationsProcessing.Transports.SQLStore
{
    public class SqlStoreReceiver : IMessageReceiver
    {
        private readonly IDataContext _context;

        // TODO {a.rechkalov, 06.05.2015}: MessageFlowMetadata sourceFlowMetadata - фильтрация, выборка только записей для текущего потока
        public SqlStoreReceiver(IDataContext context)
        {
            _context = context;
        }

        public IReadOnlyList<IMessage> Peek()
        {
            PerformedOperationsFinalProcessingMessage[] result;
            using (var scope = new TransactionScope(TransactionScopeOption.Required))
            {
                result = Query(_context.GetTable<PerformedOperationFinalProcessing>()).ToArray();
                scope.Complete();
            }

            return result;
        }

        public void Complete(IEnumerable<IMessage> successfullyProcessedMessages, IEnumerable<IMessage> failedProcessedMessages)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required))
            {
                Complete(successfullyProcessedMessages.Cast<PerformedOperationsFinalProcessingMessage>(),
                         failedProcessedMessages.Cast<PerformedOperationsFinalProcessingMessage>());

                scope.Complete();
            }
        }

        private void Complete(IEnumerable<PerformedOperationsFinalProcessingMessage> successfullyProcessedMessages,
                              IEnumerable<PerformedOperationsFinalProcessingMessage> failedProcessedMessages)
        {
            foreach (var message in successfullyProcessedMessages.SelectMany(message => message.FinalProcessings))
            {
                _context.Delete(message);
            }

            // FIXME {all, 05.05.2015}: Что делать при ошибках во время обработки?
            // Считаю, что нельзя повторно вызывать обработку сообщений - будет нарушен порядок.
            // С другой стороны, если пропустить сообщение - тоже порядок нарушается. Стоит полностью останавливать репликацию?
            foreach (var message in failedProcessedMessages.SelectMany(message => message.FinalProcessings))
            {
                _context.Delete(message);
            }
        }

        private static IEnumerable<PerformedOperationsFinalProcessingMessage> Query(IQueryable<PerformedOperationFinalProcessing> messages)
        {
            return from operation in messages
                   group operation by new { operation.EntityId, operation.EntityTypeId }
                       into operationsGroup
                       let operationsGroupKey = operationsGroup.Key
                       let maxAttempt = operationsGroup.Max(processing => processing.AttemptCount)
                       orderby maxAttempt
                       select new PerformedOperationsFinalProcessingMessage
                       {
                           EntityId = operationsGroupKey.EntityId,
                           EntityName = EntityType.Instance.Parse(operationsGroupKey.EntityTypeId),
                           MaxAttemptCount = maxAttempt,
                           FinalProcessings = operationsGroup,
                       };
        }
    }
}
