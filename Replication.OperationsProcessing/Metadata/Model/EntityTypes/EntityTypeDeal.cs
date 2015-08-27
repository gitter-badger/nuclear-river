using System;

using NuClear.AdvancedSearch.Replication.Model;
using NuClear.Model.Common.Entities;

namespace NuClear.Replication.OperationsProcessing.Metadata.Model.EntityTypes
{
    [Obsolete("Сущность не имеет отношения к домену поиска, удалить после рефакторинга на стороне ERM")]
    public sealed class EntityTypeDeal : EntityTypeBase<EntityTypeDeal>
    {
        public override int Id
        {
            get { return EntityTypeIds.Deal; }
        }

        public override string Description
        {
            get { return "Deal"; }
        }
    }
}