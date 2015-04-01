using System.Collections.Generic;

namespace NuClear.AdvancedSearch.ServiceBus.Contracts.DTO
{
    public sealed class ChangesDescriptor
    {
        public long Id { get; set; }
        public IList<ChangesDetail> Details { get; set; }
    }
}