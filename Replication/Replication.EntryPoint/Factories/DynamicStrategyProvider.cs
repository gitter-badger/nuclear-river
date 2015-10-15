using System;
using System.Collections.Generic;

using NuClear.Storage.LinqToDB.Cud;

namespace NuClear.Replication.EntryPoint.Factories
{
    public sealed class DynamicStrategyProvider : ICudStrategyProvider
    {
        private readonly ICudStrategy _iterative = new IterativeStrategy();
        private readonly ICudStrategy _bulk = new BulkStrategy();

        public ICudStrategy GetStrategy(IReadOnlyCollection<object> objects)
        {
            return objects.Count > 10
                       ? _bulk
                       : _iterative;
        }

        public void Feedback(IReadOnlyCollection<object> objects, ICudStrategy decision, long cost)
        {
        }
    }
}
