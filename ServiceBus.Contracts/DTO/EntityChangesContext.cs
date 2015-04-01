using System.Collections.Generic;

namespace NuClear.AdvancedSearch.ServiceBus.Contracts.DTO
{
    public sealed class EntityChangesContext
    {
        public IDictionary<string, IDictionary<long, ChangesDescriptor>> Store { get; set; }
    }
}