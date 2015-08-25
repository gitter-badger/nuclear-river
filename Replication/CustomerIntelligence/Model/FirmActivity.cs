using System;

using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model
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
            return obj is FirmBalance && Equals((FirmBalance)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return FirmId.GetHashCode();
            }
        }

        private bool Equals(FirmBalance other)
        {
            return FirmId == other.FirmId;
        }
    }
}