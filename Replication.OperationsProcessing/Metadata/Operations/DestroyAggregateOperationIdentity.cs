using System;

using NuClear.Model.Common.Operations.Identity;

namespace NuClear.Replication.OperationsProcessing.Metadata.Operations
{
    class DestroyAggregateOperationIdentity : OperationIdentityBase<DestroyAggregateOperationIdentity>
    {
        public override int Id { get { return 0; } }

        public Guid Guid
        {
            get { return Guid.Parse("97dbac08-cb4f-40d9-b003-a7b8484a839a"); }
        }

        public override string Description
        {
            get { return "Операция удаления агрегата"; }
        }
    }
}