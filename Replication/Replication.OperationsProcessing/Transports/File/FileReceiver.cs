using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

using NuClear.Messaging.API.Flows.Metadata;
using NuClear.Messaging.API.Receivers;
using NuClear.OperationsProcessing.API.Primary;

namespace NuClear.Replication.OperationsProcessing.Transports.File
{
    public sealed class FileReceiver : MessageReceiverBase<FileContentMessage, IPerformedOperationsReceiverSettings>
    {
        private static readonly ConcurrentDictionary<Guid, Tuple<string, FileStream>> OpenStreams = new ConcurrentDictionary<Guid, Tuple<string, FileStream>>();
        private readonly string _fileName;

        public FileReceiver(
            MessageFlowMetadata sourceFlowMetadata,
            IPerformedOperationsReceiverSettings messageReceiverSettings)
            : base(sourceFlowMetadata, messageReceiverSettings)
        {
            // todo: пока не придумал ничего лучше
            _fileName = @"C:\dev\erm\CompositionRoots\Source\2Gis.Erm.API.WCF.OrderValidation\orderValidation.config";
        }

        protected override IReadOnlyList<FileContentMessage> Peek()
        {
            if (!System.IO.File.Exists(_fileName))
            {
                return new FileContentMessage[0];
            }

            var message = new FileContentMessage();
            message.ContentProvider = () => OpenStreams.GetOrAdd(message.Id, id => Tuple.Create(_fileName, System.IO.File.Open(_fileName, FileMode.Open, FileAccess.Read, FileShare.None))).Item2;
            return new[] { message };
        }

        protected override void Complete(IEnumerable<FileContentMessage> successfullyProcessedMessages, IEnumerable<FileContentMessage> failedProcessedMessages)
        {
            foreach (var message in successfullyProcessedMessages)
            {
                Tuple<string, FileStream> tuple;
                if (OpenStreams.TryRemove(message.Id, out tuple))
                {
                    tuple.Item2.Close();
                    System.IO.File.Delete(tuple.Item1);
                }
            }

            foreach (var message in failedProcessedMessages)
            {
                Tuple<string, FileStream> tuple;
                if (OpenStreams.TryRemove(message.Id, out tuple))
                {
                    tuple.Item2.Close();
                }
            }
        }
    }
}
