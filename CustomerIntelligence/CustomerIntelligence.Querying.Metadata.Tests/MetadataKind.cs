using System;

namespace NuClear.CustomerIntelligence.Querying.Metadata.Tests
{
    [Flags]
    internal enum MetadataKind
    {
        Identity = 1,
        Elements = 2,
        Features = 4,
    }
}