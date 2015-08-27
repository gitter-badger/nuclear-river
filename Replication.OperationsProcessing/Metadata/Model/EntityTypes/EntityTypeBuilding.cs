using System;

using NuClear.Model.Common.Entities;

namespace NuClear.Replication.OperationsProcessing.Metadata.Model.EntityTypes
{
    [Obsolete("Сущность не имеет отношения к домену поиска, удалить после рефакторинга на стороне ERM")]
    public class EntityTypeBuilding : EntityTypeBase<EntityTypeBuilding>
    {
        public override int Id
        {
            get { return EntityTypeIds.Building; }
        }

        public override string Description
        {
            get { return "Building"; }
        }
    }
}