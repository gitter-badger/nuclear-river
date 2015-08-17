using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Erm
{
    public sealed class ActivityReference<T> : IErmValueObject
    {
        public long AppointmentId { get; set; }
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

            return obj is ActivityReference<T> && Equals((ActivityReference<T>)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = AppointmentId.GetHashCode();
                hashCode = (hashCode * 397) ^ Reference;
                hashCode = (hashCode * 397) ^ ReferencedType;
                hashCode = (hashCode * 397) ^ ReferencedObjectId.GetHashCode();
                return hashCode;
            }
        }

        private bool Equals(ActivityReference<T> other)
        {
            return AppointmentId == other.AppointmentId && Reference == other.Reference && ReferencedType == other.ReferencedType && ReferencedObjectId == other.ReferencedObjectId;
        }
    }
}