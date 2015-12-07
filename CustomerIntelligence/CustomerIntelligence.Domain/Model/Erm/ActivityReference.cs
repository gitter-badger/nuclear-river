namespace NuClear.CustomerIntelligence.Domain.Model.Erm
{
    public abstract class ActivityReference : IErmValueObject
    {
        public long ActivityId { get; set; }
        public int Reference { get; set; }
        public int ReferencedType { get; set; }
        public long ReferencedObjectId { get; set; }
    }
}