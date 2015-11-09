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
        private static ArrangeMetadataElement MinimalFirmAggregate
            => ArrangeMetadataElement.Config
                .Name(nameof(MinimalFirmAggregate))
                .IncludeSharedDictionary(CategoryDictionary)
                .CustomerIntelligence(
                    new CI::Firm { Id = 1, ClientId = null, CreatedOn = DateTimeOffset.MinValue, AddressCount = 1, CategoryGroupId = 0, Name = "FirmName", HasPhone = false, HasWebsite = false, LastDisqualifiedOn = null, LastDistributedOn = null, ProjectId = 1, OwnerId = 27 },
                    new CI::FirmCategory { FirmId = 1, CategoryId = 1 },
                    new CI::FirmCategory { FirmId = 1, CategoryId = 2 },
                    new CI::FirmCategory { FirmId = 1, CategoryId = 3 },
                    new CI::FirmCategory { FirmId = 1, CategoryId = 4 },
                    new CI::FirmActivity { FirmId = 1, LastActivityOn = null },
                    new CI::Project { Id = 1, Name = "ProjectOne" })
                .Fact(
                    new Facts::Firm { Id = 1, ClientId = null, CreatedOn = DateTimeOffset.MinValue, LastDisqualifiedOn = null, Name = "FirmName", OrganizationUnitId = 1, OwnerId = 27 },
                    new Facts::FirmAddress { Id = 1, FirmId = 1, TerritoryId = 1 },
                    new Facts::CategoryFirmAddress { Id = 1, CategoryId = 3, FirmAddressId = 1 },
                    new Facts::CategoryFirmAddress { Id = 2, CategoryId = 4, FirmAddressId = 1 },
                    new Facts::Project { Id = 1, Name = "ProjectOne", OrganizationUnitId = 1 })
                .Erm(
                    new Erm::Firm { Id = 1, ClientId = null, ClosedForAscertainment = false, CreatedOn = DateTimeOffset.MinValue, IsActive = true, IsDeleted = false, LastDisqualifyTime = null, Name = "FirmName", OrganizationUnitId = 1, OwnerId = 27 },
                    new Erm::FirmAddress { Id = 1, FirmId = 1, TerritoryId = 1, ClosedForAscertainment = false, IsActive = true, IsDeleted = false },
                    new Erm::CategoryFirmAddress { Id = 1, CategoryId = 3, FirmAddressId = 1, IsActive = true, IsDeleted = false },
                    new Erm::CategoryFirmAddress { Id = 2, CategoryId = 4, FirmAddressId = 1, IsActive = true, IsDeleted = false },
                    new Erm::Project { Id = 1, IsActive = true, Name = "ProjectOne", OrganizationUnitId = 1 })
                .Bit(
                    new Bit::ProjectCategoryStatistics { ProjectId = 1, CategoryId = 3, AdvertisersCount = 2 },
                    new Bit::FirmCategoryStatistics { FirmId = 1, CategoryId = 3, ProjectId = 1, Hits = 10, Shows = 20 },
                    new Bit::FirmCategory { FirmId = 1, CategoryId = 3, ProjectId = 1 },
                    new Bit::FirmCategory { FirmId = 1, CategoryId = 4, ProjectId = 1 })
                .Statistics(
                    new Statistics::FirmCategoryStatistics { FirmId = 1, CategoryId = 1, ProjectId = 1, AdvertisersShare = null, Hits = null, Shows = null, FirmCount = null },
                    new Statistics::FirmCategoryStatistics { FirmId = 1, CategoryId = 2, ProjectId = 1, AdvertisersShare = null, Hits = null, Shows = null, FirmCount = null },
                    new Statistics::FirmCategoryStatistics { FirmId = 1, CategoryId = 3, ProjectId = 1, Hits = 10, Shows = 20, AdvertisersShare = 1, FirmCount = 1 },
                    new Statistics::FirmCategoryStatistics { FirmId = 1, CategoryId = 4, ProjectId = 1, Hits = 0, Shows = 0, AdvertisersShare = 0, FirmCount = 1 });
    }
}
