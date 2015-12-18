namespace NuClear.ValidationRules.Domain.Model.Facts
{
    public sealed class Category : IErmFactObject
    {
        public long Id { get; set; }
        public long ParentId { get; set; }
    }
}
