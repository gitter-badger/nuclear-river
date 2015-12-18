namespace NuClear.ValidationRules.Domain.Model.Facts
{
    public sealed class AssociatedPositionsGroup : IErmFactObject
    {
        public long Id { get; set; }
        public long PricePositionId { get; set; }
    }
}
