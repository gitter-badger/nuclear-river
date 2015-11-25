namespace NuClear.CustomerIntelligence.Domain.Model.Erm
{
    public sealed class Territory : IErmObject
    {
        public Territory()
        {
            IsActive = true;
        }

        public long Id { get; set; }

        public string Name { get; set; }

        public long OrganizationUnitId { get; set; }

        public bool IsActive { get; set; }
    }
}