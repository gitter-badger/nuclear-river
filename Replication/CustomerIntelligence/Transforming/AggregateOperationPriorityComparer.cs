using System;
using System.Collections.Generic;

using NuClear.AdvancedSearch.Replication.API.Operations;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    internal sealed class AggregateOperationPriorityComparer : IComparer<Type>
    {
        private static readonly IReadOnlyDictionary<Type, int> Priority
            = new Dictionary<Type, int>
              {
                  { typeof(DestroyAggregate), 3 },
                  { typeof(InitializeAggregate), 2 },
                  { typeof(RecalculateAggregate), 1 },
              };

        public int Compare(Type x, Type y)
        {
            return Priority[x] - Priority[y];
        }
    }
}