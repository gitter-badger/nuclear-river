namespace NuClear.AdvancedSearch.Replication.API.Transforming.Facts
{
    public class FactDependencyInfoBuilder
    {
        public static IFactDependencyInfo Create<TAggregate>()
        {
            return new DirectAggregateDependencyInfo<TAggregate>();
        }

        public static IFactDependencyInfo Create<TAggregate>(MapToDependentAggregateSpecProvider dependentAggregateSpecProvider)
        {
            return new AggregateDependencyInfo<TAggregate>(dependentAggregateSpecProvider);
        }
    }
}