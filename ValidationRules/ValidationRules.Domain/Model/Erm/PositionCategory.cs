namespace NuClear.ValidationRules.Domain.Model.Erm
{
    public sealed class PositionCategory
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
