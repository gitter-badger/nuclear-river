using System.Collections.Generic;

namespace NuClear.AdvancedSearch.ServiceBus.Contracts.DTO
{
    public sealed class TrackedUseCase
    {
        public IList<OperationScopeNode> Operations { get; set; }
    }
}
