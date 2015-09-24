using System;

namespace NuClear.CustomerIntelligence.Domain.Model.CI
{
    public sealed class FirmActivity : ICustomerIntelligenceObject
    {
        public long FirmId { get; set; }

        public DateTimeOffset? LastActivityOn { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            return obj is FirmActivity && Equals((FirmActivity)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return FirmId.GetHashCode();
            }
        }

        private bool Equals(FirmActivity other)
        {
            return FirmId == other.FirmId;
        }
    }
}