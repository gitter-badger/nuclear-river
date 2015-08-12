using System;

using NuClear.Model.Common.Operations.Identity;

namespace NuClear.Replication.OperationsProcessing.Metadata.Operations
{
    internal sealed class StatisticsOperationIdentity : OperationIdentityBase<StatisticsOperationIdentity>
    {
        public override int Id { get { return 0; } }

        public Guid Guid
        {
            get { return Guid.Parse("4AE6D197-0F8A-4A72-B64B-5ED96C781EC4"); }
        }

        public override string Description
        {
            get { return "Операция пересчёта статистики фирмы-проекта-рубрики"; }
        }
    }
}