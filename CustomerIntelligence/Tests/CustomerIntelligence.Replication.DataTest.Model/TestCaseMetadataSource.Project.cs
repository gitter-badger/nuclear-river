using System;

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
        private static readonly ArrangeMetadataElement ProjectAggregate
            = ArrangeMetadataElement.Config
                .Name(nameof(ProjectAggregate))
                .CustomerIntelligence(
                    new CI::Project { Id = 1, Name = "ProjectOne" },
                    new CI::ProjectCategory { ProjectId = 1, CategoryId = 1, Name = "Category Name", Level = 1, ParentId = null },
                    new CI::CategoryGroup {Id = 1, Name = "CategoryGroup Name", Rate = 1})
                .Fact(
                    new Facts::Project { Id = 1, Name = "ProjectOne", OrganizationUnitId = 1 },
                    new Facts::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 },
                    new Facts::CategoryOrganizationUnit { Id = 1, CategoryId = 1, CategoryGroupId = 1, OrganizationUnitId = 1 },
                    new Facts::Category { Id = 1, Level = 1, Name = "Category Name", ParentId = null },
                    new Facts::CategoryGroup { Id = 1, Name = "CategoryGroup Name", Rate = 1 }
                    )
                .Erm(
                    new Erm::Project { Id = 1, IsActive = true, Name = "ProjectOne", OrganizationUnitId = 1 },
                    new Erm::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1, IsActive = true, IsDeleted = false },
                    new Erm::CategoryOrganizationUnit { Id = 1, CategoryId = 1, CategoryGroupId = 1, OrganizationUnitId = 1, IsActive = true, IsDeleted = false},
                    new Erm::Category { Id = 1, Level = 1, Name = "Category Name", ParentId = null, IsActive = true, IsDeleted = false},
                    new Erm::CategoryGroup { Id = 1, Name = "CategoryGroup Name", Rate = 1, IsActive = true, IsDeleted = false });

    }
}
