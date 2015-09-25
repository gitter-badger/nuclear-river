using System;

namespace NuClear.Querying.QueryExecution
{
    public sealed class MetadataIdentityAnnotation
    {
        public MetadataIdentityAnnotation(Uri value)
        {
            Value = value;
        }

        public Uri Value { get; set; }
    }
}