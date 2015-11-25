using System;

using NuClear.AdvancedSearch.Common.Metadata.Model;

namespace NuClear.CustomerIntelligence.Domain.Model.Facts
{
    public sealed class Order : IFactObject
    {
        public long Id { get; set; }

        public DateTimeOffset EndDistributionDateFact { get; set; }

        public long FirmId { get; set; }
    }
}