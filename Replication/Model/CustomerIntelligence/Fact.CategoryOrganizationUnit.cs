namespace NuClear.AdvancedSearch.Replication.Model.CustomerIntelligence
{
    public static partial class Fact
    {
        public sealed class CategoryOrganizationUnit : IIdentifiable
        {
            public long Id { get; set; }

            public long CategoryId { get; set; }

            public long CategoryGroupId { get; set; }

            public long OrganizationUnitId { get; set; }
        }
    }
}