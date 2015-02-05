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
        public bool HasWebsite { get; set; }
        public bool HasPhone { get; set; }
        public long? CategoryGroup { get; set; }
        public int AddressCount { get; set; }

        public OrganizationUnit OrganizationUnit { get; set; }
        public Territory Territory { get; set; }
        public Client Client { get; set; }
        public ICollection<FirmCategory> Categories { get; set; }
    }
}