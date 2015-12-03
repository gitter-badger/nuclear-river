using NuClear.AdvancedSearch.Common.Metadata.Model;

namespace NuClear.CustomerIntelligence.Domain.Model.Facts
{
    public sealed class LegalPerson : IFactObject
    {
        public long Id { get; set; }

        public long ClientId { get; set; }
    }
}