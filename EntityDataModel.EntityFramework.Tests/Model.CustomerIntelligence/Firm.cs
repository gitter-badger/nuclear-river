using System;
using System.Collections.Generic;

namespace NuClear.AdvancedSearch.EntityDataModel.EntityFramework.Tests.Model.CustomerIntelligence
{
    public class Firm
    {
        public long Id { get; set; }
        public long OrganizationUnitId { get; set; }
        public long TerritoryId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastQualifiedOn { get; set; }
        public DateTime LastDistributedOn { get; set; }
        public bool HasWebsite { get; set; }
        public bool HasPhone { get; set; }
        public byte CategoryGroup { get; set; }
        public int AddressCount { get; set; }

        public Client Client { get; set; }
        public ICollection<Category> Categories { get; set; }
    }
}