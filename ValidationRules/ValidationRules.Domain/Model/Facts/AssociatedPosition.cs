namespace NuClear.ValidationRules.Domain.Model.Facts
{
    public sealed class AssociatedPosition : IErmFactObject
    {
        public long Id { get; set; }
        public long AssociatedPositionsGroupId { get; set; }
        public long PositionId { get; set; }
        public int ObjectBindingType { get; set; }
    }
}
