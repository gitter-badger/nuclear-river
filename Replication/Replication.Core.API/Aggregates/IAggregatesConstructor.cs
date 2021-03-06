﻿using System.Collections.Generic;

using NuClear.AdvancedSearch.Common.Metadata.Model.Operations;

namespace NuClear.Replication.Core.API.Aggregates
{
    public interface IAggregatesConstructor
    {
        void Construct(IEnumerable<AggregateOperation> operations);
    }
}