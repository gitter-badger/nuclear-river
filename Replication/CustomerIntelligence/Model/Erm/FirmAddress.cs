using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Erm
{
    public sealed class FirmAddress : IIdentifiable
    {
        public FirmAddress()
        {
            IsActive = true;
        }

        public long Id { get; set; }

        public long FirmId { get; set; }

        public bool ClosedForAscertainment { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }
    }
}