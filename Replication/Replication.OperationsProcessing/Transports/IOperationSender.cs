using System.Collections.Generic;

using NuClear.AdvancedSearch.Common.Metadata.Model;

namespace NuClear.Replication.OperationsProcessing.Transports
{
    public interface IOperationSender<in TOperation>
        where TOperation : IOperation
    {
        void Push(IEnumerable<TOperation> operations);
    }
}