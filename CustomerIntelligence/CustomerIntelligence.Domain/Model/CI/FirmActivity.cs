using System;

namespace NuClear.CustomerIntelligence.Domain.Model.CI
{
    public sealed class FirmActivity : ICustomerIntelligenceObject
    {
        public long FirmId { get; set; }

        public DateTimeOffset? LastActivityOn { get; set; }
    }
}