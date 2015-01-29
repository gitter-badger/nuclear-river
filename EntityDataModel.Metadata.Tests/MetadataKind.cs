using System;

namespace NuClear.AdvancedSearch.EntityDataModel.Metadata.Tests
{
    [Flags]
    internal enum MetadataKind
    {
        Identity = 1,
        Elements = 2,
        Features = 4,
    }
}