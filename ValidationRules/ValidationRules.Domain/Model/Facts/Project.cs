namespace NuClear.ValidationRules.Domain.Model.Facts
{
    public sealed class Project : IErmFactObject
    {
        public long Id { get; set; }
        public long OrganizationUnitId { get; set; }
    }
}
