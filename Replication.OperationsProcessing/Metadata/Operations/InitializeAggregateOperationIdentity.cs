using System;

using NuClear.Model.Common.Operations.Identity;

namespace NuClear.Replication.OperationsProcessing.Metadata.Operations
{
    internal sealed class InitializeAggregateOperationIdentity : OperationIdentityBase<InitializeAggregateOperationIdentity>
    {
        public override int Id { get { return 0; } }

        public Guid Guid
        {
            get { return Guid.Parse("2eca8d09-de8f-48e6-9634-a686da3f45f9"); }
        }

        public override string Description
        {
            get { return "Операция инициализации агрегата"; }
        }
    }
}