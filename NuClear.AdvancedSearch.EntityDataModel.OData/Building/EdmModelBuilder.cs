using System;

using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;

namespace NuClear.AdvancedSearch.EntityDataModel.OData.Building
{
    public sealed class EdmModelBuilder
    {
        private readonly IEdmModelSource _source;

        public EdmModelBuilder(IEdmModelSource source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            _source = source;
        }

        public IEdmModel Build()
        {
            var ns = _source.Namespace;

            var books = new EdmEntityType(ns, "Book");
            var id = books.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Int64, false);
            books.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            books.AddKeys(id);

            var container = new EdmEntityContainer(ns, "Container");
            container.AddEntitySet("Books", books);

            var model = new EdmModel();
            model.AddElement(container);
            model.AddElement(books);

            return model;
        }
    }
}
