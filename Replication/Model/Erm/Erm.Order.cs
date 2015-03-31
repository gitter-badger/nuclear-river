using System;

// ReSharper disable once CheckNamespace
namespace NuClear.AdvancedSearch.Replication.Model
{
    public static partial class Erm
    {
        public sealed class Order : IIdentifiable
        {
            public Order()
            {
                IsActive = true;
            }

            public long Id { get; set; }

            public DateTimeOffset EndDistributionDateFact { get; set; }

            public int WorkflowStepId { get; set; }

            public long FirmId { get; set; }

            public bool IsActive { get; set; }

            public bool IsDeleted { get; set; }
        }
    }
}