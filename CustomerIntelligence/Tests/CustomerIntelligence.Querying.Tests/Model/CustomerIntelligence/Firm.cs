﻿using System;
using System.Collections.Generic;

namespace NuClear.CustomerIntelligence.Querying.Tests.Model.CustomerIntelligence
{
    public class Firm
    {
        public long Id { get; set; }
        public long ProjectId { get; set; }
        public string Name { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset? LastDisqualifiedOn { get; set; }
        public DateTimeOffset? LastDistributedOn { get; set; }
        public DateTimeOffset? LastActivityOn { get; set; }
        public bool HasPhone { get; set; }
        public bool HasWebsite { get; set; }
        public int AddressCount { get; set; }

        public long OwnerId { get; set; }

        public ICollection<FirmBalance> Balances { get; set; }
        public ICollection<FirmCategory> Categories { get; set; }
        public CategoryGroup CategoryGroup { get; set; }
        public Client Client { get; set; }
        public ICollection<FirmTerritory> Territories { get; set; }
    }
}