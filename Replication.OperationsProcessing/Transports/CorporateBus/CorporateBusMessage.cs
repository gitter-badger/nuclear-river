using System;
using System.Collections.Generic;

using NuClear.Messaging.API;
using NuClear.Messaging.Transports.CorporateBus.API;

namespace NuClear.Replication.OperationsProcessing.Transports.CorporateBus
{
    public sealed class CorporateBusPerformedOperationsMessage : MessageBase
    {
        private readonly Guid _guid;
        private readonly IEnumerable<CorporateBusPackage> _packages;

        public CorporateBusPerformedOperationsMessage(IEnumerable<CorporateBusPackage> packages)
        {
            _guid = Guid.NewGuid();
            _packages = packages;
        }

        public override Guid Id { get { return _guid; } }
        public IEnumerable<CorporateBusPackage> Packages { get { return _packages; } }
    }
}