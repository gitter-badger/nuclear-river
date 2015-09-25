using System;

using NuClear.Model.Common.Operations.Identity;

namespace NuClear.Replication.OperationsProcessing.Identities.Operations
{
    public sealed class RecalculateAggregateOperationIdentity : OperationIdentityBase<RecalculateAggregateOperationIdentity>
    {
        public override int Id { get { return 0; } }

        public Guid Guid
        {
            get { return Guid.Parse("391a4637-388d-480f-8872-4587f167a23d"); }
        }

        public override string Description
        {
            get { return "Операция пересчёта агрегата"; }
        }
    }
}