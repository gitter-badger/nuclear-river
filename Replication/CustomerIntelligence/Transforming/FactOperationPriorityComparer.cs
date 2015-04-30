using System;
using System.Collections.Generic;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    public sealed class FactOperationPriorityComparer : IComparer<FactOperation>
    {
        private static readonly IReadOnlyDictionary<Type, int> Priority
            = new Dictionary<Type, int>
              {
                  { typeof(CreateFact), 3 },
                  { typeof(UpdateFact), 2 },
                  { typeof(DeleteFact), 1 },
              };

        public int Compare(FactOperation x, FactOperation y)
        {
            return Priority[x.GetType()] - Priority[y.GetType()];
        }
    }
}