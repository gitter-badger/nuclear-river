using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.OData;

using Microsoft.OData.Edm;

namespace NuClear.AdvancedSearch.QueryExecution
{
    public static class EdmModelExtensions
    {
        public static IEdmModel AddClrAnnotations(this IEdmModel model, IEnumerable<Type> clrTypes)
        {
            var tuples = model.SchemaElements.Join(clrTypes, x => x.Name, x => x.Name, Tuple.Create, StringComparer.OrdinalIgnoreCase);

            foreach (var tuple in tuples)
            {
                var annotation = new ClrTypeAnnotation(tuple.Item2);
                model.SetAnnotationValue(tuple.Item1, annotation);
            }

            return model;
        }
    }
}