﻿using System.Collections.Generic;

using NuClear.AdvancedSearch.Common.Metadata.Model;

namespace NuClear.Replication.Core.API.Facts
{
    public interface IStatisticsImporter
    {
        IReadOnlyCollection<IOperation> Import(IDataTransferObject dto);
    }
}