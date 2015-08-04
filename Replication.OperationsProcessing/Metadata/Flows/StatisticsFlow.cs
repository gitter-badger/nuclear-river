using System;

using NuClear.Messaging.API.Flows;

namespace NuClear.Replication.OperationsProcessing.Metadata.Flows
{
    public sealed class StatisticsFlow : MessageFlowBase<StatisticsFlow>
    {
        public override Guid Id
        {
            get { return new Guid("EED0A445-4A53-4D49-89F5-01DD440C85C8"); }
        }

        public override string Description
        {
            get { return "Поток изменений, вызывающих пересчёт статистики по рубрике проекта"; }
        }
    }
}