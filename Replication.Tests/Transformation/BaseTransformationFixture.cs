using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;

namespace NuClear.AdvancedSearch.Replication.Tests.Transformation
{
    internal abstract class BaseTransformationFixture : BaseDataFixture
    {
        #region Fact

        protected static class Fact
        {
            public static FactOperation Create<T>(long entityId)
            {
                return new FactOperation(typeof(T), entityId);
            }

            public static FactOperation Update<T>(long entityId)
            {
                return new FactOperation(typeof(T), entityId);
            }

            public static FactOperation Delete<T>(long entityId)
            {
                return new FactOperation(typeof(T), entityId);
            }
        }

        #endregion

        #region Aggregate

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

        #endregion
    }
}