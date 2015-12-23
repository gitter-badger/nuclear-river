using System.IO;

using NuClear.AdvancedSearch.Common.Metadata.Model;

namespace NuClear.AdvancedSearch.Common.Metadata
{
    public interface IConfigParser
    {
        IDataTransferObject Parse(Stream config);
    }
}
