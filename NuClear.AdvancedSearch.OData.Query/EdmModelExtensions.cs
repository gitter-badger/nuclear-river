using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.OData;

using Microsoft.OData.Edm;

namespace NuClear.AdvancedSearch.OData.Query
{
    public static class EdmModelExtensions
    {
        public static IEdmModel AddClrAnnotations(this IEdmModel model, IEnumerable<Type> clrTypes)
        {
            var tuples = model.SchemaElements.OfType<IEdmTerm>().Join(clrTypes, x => x.Name, x => x.Name, Tuple.Create, StringComparer.OrdinalIgnoreCase);

            foreach (var tuple in tuples)
            {
                var annotation = new ClrTypeAnnotation(tuple.Item2);
                model.SetAnnotationValue(tuple.Item1, annotation);

                var structuredType = tuple.Item1 as IEdmStructuredType;
                if (structuredType != null)
                {
                    AddClrPropertyAnnotations(model, structuredType, tuple.Item2);
                }
            }
            
            return model;
        }

        private static void AddClrPropertyAnnotations(IEdmModel model, IEdmStructuredType edmType, Type clrType)
        {
            var tuples = edmType.DeclaredProperties.Join(clrType.GetProperties(), x => x.Name, x => x.Name, Tuple.Create, StringComparer.OrdinalIgnoreCase);
            foreach (var tuple in tuples)
            {
                var annotation = new ClrPropertyInfoAnnotation(tuple.Item2);
                model.SetAnnotationValue(tuple.Item1, annotation);
            }
        }
    }
}