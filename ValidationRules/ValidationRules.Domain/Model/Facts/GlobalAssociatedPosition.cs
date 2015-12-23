namespace NuClear.ValidationRules.Domain.Model.Facts
{
    public sealed class GlobalAssociatedPosition : IConfigFactObject
    {
        public long MasterPositionId { get; set; }
        public long AssociatedPositionId { get; set; }
        public int ObjectBindingType { get; set; }
    }
}
