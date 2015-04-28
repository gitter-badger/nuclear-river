using System;

using NuClear.Messaging.API.Flows;

namespace NuClear.Replication.OperationsProcessing.Metadata.Flows
{
    public sealed class FakeFlow : MessageFlowBase<FakeFlow>
    {
        public override Guid Id
        {
            get { return new Guid("42f9c8d3-643e-4ca2-b436-1a11cc622c12"); }
        }

        public override string Description
        {
            get { return "Маркер для потока выполненных операций в системе обеспечивающих репликацию изменений в домен Customer Intelligence"; }
        }
    }
}