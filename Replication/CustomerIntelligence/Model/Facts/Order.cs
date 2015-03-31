using System;

using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts
{
    public sealed class Order : IIdentifiable
    {
        public long Id { get; set; }

        public DateTimeOffset EndDistributionDateFact { get; set; }

        public int WorkflowStepId { get; set; }

        public long FirmId { get; set; }
    }
}