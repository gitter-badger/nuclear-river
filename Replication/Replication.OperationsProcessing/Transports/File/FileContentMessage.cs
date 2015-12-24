using System;
using System.IO;

using NuClear.Messaging.API;

namespace NuClear.Replication.OperationsProcessing.Transports.File
{
    public sealed class FileContentMessage : MessageBase
    {
        public override Guid Id { get; }
            = Guid.NewGuid();

        public Func<Stream> ContentProvider { get; set; }
    }
}