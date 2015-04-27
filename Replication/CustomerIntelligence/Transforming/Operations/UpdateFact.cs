using System;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations
{
    public sealed class UpdateFact : FactOperation
    {
        public UpdateFact(Type factType, long factId)
            : base(factType, factId)
        {
            Priority = 2;
        }
    }
}