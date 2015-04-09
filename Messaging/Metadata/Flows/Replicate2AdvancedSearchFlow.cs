using System;

using NuClear.Messaging.API.Flows;

namespace NuClear.AdvancedSearch.Messaging.Metadata.Flows
{
    public sealed class Replicate2AdvancedSearchFlow : MessageFlowBase<Replicate2AdvancedSearchFlow>
    {
        public override Guid Id
        {
            get { return new Guid("9F2C5A2A-924C-485A-9790-9066631DB307"); }
        }

        public override string Description
        {
            get { return "Маркер для потока выполненных операций в системе обеспечивающих репликацию изменений в Advanced Search"; }
        }
    }
}