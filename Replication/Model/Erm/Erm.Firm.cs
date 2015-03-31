using System;

// ReSharper disable once CheckNamespace
namespace NuClear.AdvancedSearch.Replication.Model
{
    public static partial class Erm
    {
        public sealed class Firm : IIdentifiable
        {
            public Firm()
            {
                IsActive = true;
            }

            public long Id { get; set; }

            public string Name { get; set; }

            public DateTimeOffset CreatedOn { get; set; }

            public DateTimeOffset? LastDisqualifyTime { get; set; }

            public long? ClientId { get; set; }

            public long OrganizationUnitId { get; set; }

            public long TerritoryId { get; set; }

            public bool ClosedForAscertainment { get; set; }

            public bool IsActive { get; set; }

            public bool IsDeleted { get; set; }
        }
    }
}