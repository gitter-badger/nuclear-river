﻿using System.Collections;
using System.Collections.Generic;

using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.API.Transforming
{
    public interface ISourceChangesDetector
    {
        IMergeResult<T> DetectChanges<T>(MapSpecification<IEnumerable, IEnumerable<T>> mapSpec, IReadOnlyCollection<long> ids);
    }
}