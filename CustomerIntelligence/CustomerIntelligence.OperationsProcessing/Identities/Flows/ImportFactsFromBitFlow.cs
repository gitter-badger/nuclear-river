using System;

using NuClear.Messaging.Transports.CorporateBus.Flows;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.Flows
{
    public sealed class ImportFactsFromBitFlow : CorporateBusFlowBase<ImportFactsFromBitFlow>
    {
        public override Guid Id
        {
            get { return new Guid("C555F76E-E6F6-44A2-8323-1A54BDA2AF7D"); }
        }

        public override string FlowName
        {
            get { return "flowStatistics"; }
        }

        public override string Description
        {
            get { return "Поток статистики от OLAP"; }
        }
    }
}