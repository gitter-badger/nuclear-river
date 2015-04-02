using NuClear.AdvancedSearch.Replication.Transforming;

namespace NuClear.AdvancedSearch.Replication.Tests.Transformation
{
    internal abstract class BaseTransformationFixture : BaseDataFixture
    {
        #region Operation

        protected static class Operation
        {
            public static OperationInfo Create<T>(long entityId)
            {
                return Build<T>(Transforming.Operation.Created, entityId);
            }

            public static OperationInfo Update<T>(long entityId)
            {
                return Build<T>(Transforming.Operation.Updated, entityId);
            }

            public static OperationInfo Delete<T>(long entityId)
            {
                return Build<T>(Transforming.Operation.Deleted, entityId);
            }

            private static OperationInfo Build<T>(Transforming.Operation operation, long entityId)
            {
                return new OperationInfo(operation, typeof(T), entityId);
            }
        }

        #endregion
    }
}