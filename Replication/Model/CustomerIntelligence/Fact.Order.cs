using System;

namespace NuClear.AdvancedSearch.Replication.Model.CustomerIntelligence
{
    public static partial class Fact
    {
        public sealed class Order : IIdentifiable
        {
            public long Id { get; set; }
            public DateTimeOffset EndDistributionDateFact { get; set; }
            public int WorkflowStepId { get; set; }
            
            public long FirmId { get; set; }
        }
    }
}