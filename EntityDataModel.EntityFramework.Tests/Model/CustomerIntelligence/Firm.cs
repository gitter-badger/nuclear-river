using System;
using System.Collections.Generic;

namespace NuClear.AdvancedSearch.EntityDataModel.EntityFramework.Tests.Model.CustomerIntelligence
{
    public class Firm
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset? LastDisqualifiedOn { get; set; }
        public DateTimeOffset? LastDistributedOn { get; set; }
        public bool HasPhone { get; set; }
        public bool HasWebsite { get; set; }
        public int AddressCount { get; set; }

        public long CategoryGroupId { get; set; }
        public long OrganizationUnitId { get; set; }
        public long TerritoryId { get; set; }

        public Client Client { get; set; }
        public ICollection<FirmAccount> Accounts { get; set; }
        public ICollection<FirmCategory> Categories { get; set; }
        public ICollection<FirmCategoryGroup> CategoryGroups { get; set; }
    }
}