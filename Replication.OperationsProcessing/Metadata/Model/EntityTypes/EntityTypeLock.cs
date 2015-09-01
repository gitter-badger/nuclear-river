using System;

using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.Model.Common.Entities;

namespace NuClear.Replication.OperationsProcessing.Metadata.Model.EntityTypes
{
    [Obsolete("Сущность не имеет отношения к домену поиска, удалить после рефакторинга на стороне ERM")]
    public sealed class EntityTypeLock : EntityTypeBase<EntityTypeLock>
    {
        public override int Id
        {
            get { return EntityTypeIds.Lock; }
        }

        public override string Description
        {
            get { return "Lock"; }
        }
    }
}