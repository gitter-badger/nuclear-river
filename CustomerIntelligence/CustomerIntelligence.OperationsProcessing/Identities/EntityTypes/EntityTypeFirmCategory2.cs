﻿using NuClear.CustomerIntelligence.Domain;
using NuClear.Model.Common.Entities;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeFirmCategory2 : EntityTypeBase<EntityTypeFirmCategory2>
    {
        public override int Id
        {
            get { return EntityTypeIds.FirmCategory2; }
        }

        public override string Description
        {
            get { return "FirmCategory2"; }
        }
    }
}