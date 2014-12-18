using System.Collections.Generic;

namespace NuClear.AdvancedSearch.EntityDataModel.OData.Building
{
    public interface IEdmModelSource
    {
        string Namespace { get; }

        IReadOnlyCollection<EdmEntityType> Entities { get; }

        IReadOnlyCollection<EdmEntityRelationInfo> Relations { get; }
    }
}