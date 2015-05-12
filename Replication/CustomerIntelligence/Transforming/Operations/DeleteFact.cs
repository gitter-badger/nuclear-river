using System;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations
{
    public sealed class DeleteFact : FactOperation
    {
        public DeleteFact(Type factType, long factId)
            : base(factType, factId)
        {
        }
    }
}