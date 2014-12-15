using System.Collections.Generic;

namespace NuClear.AdvancedSearch.EntityDataModel.OData.Building
{
    public interface IEdmModelSource
    {
        string Namespace { get; }

        IEnumerable<string> ResolveContexts();

        IEnumerable<string> ResolveContextEntities(string context);

        IEnumerable<string> ResolveEntityProperties(string entity);
    }
}