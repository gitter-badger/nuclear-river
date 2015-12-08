using System;

namespace NuClear.QueryingMetadata.Tests
{
    [Flags]
    internal enum MetadataKind
    {
        Identity = 1,
        Elements = 2,
        Features = 4,
    }
}