using System;

using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts
{
    public sealed class Client : IIdentifiable
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public DateTimeOffset? LastDisqualifiedOn { get; set; }

        public bool HasPhone { get; set; }

        public bool HasWebsite { get; set; }
    }
}