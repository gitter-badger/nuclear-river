using System;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations
{
    public sealed class CreateFact : FactOperation
    {
        public CreateFact(Type factType, long factId)
            : base(factType, factId)
        {
            Priority = 3;
        }
    }
}