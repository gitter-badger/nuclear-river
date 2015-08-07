using NuClear.AdvancedSearch.Replication.API.Operations;

namespace NuClear.AdvancedSearch.Replication.Tests.Transformation
{
    internal abstract class BaseTransformationFixture : BaseDataFixture
    {
        protected static class Fact
        {
            public static FactOperation Operation<T>(long entityId)
            {
                return new FactOperation(typeof(T), entityId);
            }
        }

        protected static class Aggregate
        {
            public static AggregateOperation Initialize<T>(long entityId)
            {
                return new InitializeAggregate(typeof(T), entityId);
            }

            public static AggregateOperation Recalculate<T>(long entityId)
            {
                return new RecalculateAggregate(typeof(T), entityId);
            }

            public static AggregateOperation Destroy<T>(long entityId)
            {
                return new DestroyAggregate(typeof(T), entityId);
            }
        }

        protected static class Statistics
        {
            public static CalculateStatisticsOperation Operation(long projectId, long? categoryId = null)
            {
                return new CalculateStatisticsOperation { ProjectId = projectId, CategoryId = categoryId };
            }
        }
    }
}