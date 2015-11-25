using NuClear.DataTest.Metamodel.Dsl;

namespace CustomerIntelligence.Replication.DataTest.Model
{
    using Bit = NuClear.CustomerIntelligence.Domain.Model.Bit;
    using CI = NuClear.CustomerIntelligence.Domain.Model.CI;
    using Erm = NuClear.CustomerIntelligence.Domain.Model.Erm;
    using Facts = NuClear.CustomerIntelligence.Domain.Model.Facts;
    using Statistics = NuClear.CustomerIntelligence.Domain.Model.Statistics;

    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ProjectAggregate
            => ArrangeMetadataElement.Config
                .Name(nameof(ProjectAggregate))
                .IncludeSharedDictionary(CategoryDictionary)
                .IncludeSharedDictionary(CategoryGroupDictionary)
                .CustomerIntelligence(
                    new CI::Project { Id = 1, Name = "ProjectOne" },
                    new CI::ProjectCategory { ProjectId = 1, CategoryId = 1, Name = "Category 1 level 1", Level = 1, ParentId = null })
                .Fact(
                    new Facts::Project { Id = 1, Name = "ProjectOne", OrganizationUnitId = 1 },
                    new Facts::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 },
                    new Facts::CategoryOrganizationUnit { Id = 1, CategoryId = 1, CategoryGroupId = 1, OrganizationUnitId = 1 })
                .Erm(
                    new Erm::Project { Id = 1, IsActive = true, Name = "ProjectOne", OrganizationUnitId = 1 },
                    new Erm::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1, IsActive = true, IsDeleted = false },
                    new Erm::CategoryOrganizationUnit { Id = 1, CategoryId = 1, CategoryGroupId = 1, OrganizationUnitId = 1, IsActive = true, IsDeleted = false});
    }
}
