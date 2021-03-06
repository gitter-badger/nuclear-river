﻿using NuClear.AdvancedSearch.Common.Metadata.Elements;

namespace NuClear.Replication.Core.API.Aggregates
{
    public interface IValueObjectProcessorFactory
    {
        IValueObjectProcessor Create(IValueObjectMetadataElement metadata);
    }
}