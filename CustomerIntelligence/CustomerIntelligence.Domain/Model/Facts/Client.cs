using System;

using NuClear.AdvancedSearch.Common.Metadata.Model;

namespace NuClear.CustomerIntelligence.Domain.Model.Facts
{
    public sealed class Client : IFactObject
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public DateTimeOffset? LastDisqualifiedOn { get; set; }

        public bool HasPhone { get; set; }

        public bool HasWebsite { get; set; }
    }
}