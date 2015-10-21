using NuClear.AdvancedSearch.Common.Metadata.Model;

namespace NuClear.CustomerIntelligence.Domain.Model.Erm
{
    public abstract class ActivityReference : IErmValueObject
    {
        public long ActivityId { get; set; }
        public int Reference { get; set; }
        public int ReferencedType { get; set; }
        public long ReferencedObjectId { get; set; }

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

            return obj != null && obj.GetType() == GetType() && Equals((ActivityReference)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = ActivityId.GetHashCode();
                hashCode = (hashCode * 397) ^ Reference;
                hashCode = (hashCode * 397) ^ ReferencedType;
                hashCode = (hashCode * 397) ^ ReferencedObjectId.GetHashCode();
                return hashCode;
            }
        }

        private bool Equals(ActivityReference other)
        {
            return ActivityId == other.ActivityId && Reference == other.Reference && ReferencedType == other.ReferencedType && ReferencedObjectId == other.ReferencedObjectId;
        }
    }
}