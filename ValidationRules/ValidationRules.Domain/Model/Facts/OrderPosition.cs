namespace NuClear.ValidationRules.Domain.Model.Facts
{
    public sealed class OrderPosition : IErmFactObject
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public long PricePositionId { get; set; }
    }
}
