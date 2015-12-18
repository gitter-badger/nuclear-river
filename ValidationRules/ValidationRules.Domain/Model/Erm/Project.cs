namespace NuClear.ValidationRules.Domain.Model.Erm
{
    public sealed class Project
    {
        public long Id { get; set; }
        public long? OrganizationUnitId { get; set; }
        public bool IsActive { get; set; }
    }
}
