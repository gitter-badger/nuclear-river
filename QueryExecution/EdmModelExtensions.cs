using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
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

        public static IEdmModel AddDbCompiledModelAnnotation(this IEdmModel model, DbCompiledModel dbCompiledModel)
        {
            var annotation = new DbCompiledModelAnnotation(dbCompiledModel);
            model.SetAnnotationValue(model, annotation);

            return model;
        }

        public static DbCompiledModel GetDbCompiledModel(this IEdmModel model)
        {
            var annotation = model.GetAnnotationValue<DbCompiledModelAnnotation>(model);
            return annotation.Value;
        }


        public static IEdmModel AddMetadataIdentityAnnotation(this IEdmModel model, Uri identity)
        {
            var annotation = new MetadataIdentityAnnotation(identity);
            model.SetAnnotationValue(model, annotation);

            return model;
        }

        public static Uri GetMetadataIdentity(this IEdmModel model)
        {
            var annotation = model.GetAnnotationValue<MetadataIdentityAnnotation>(model);
            return annotation.Value;
        }
    }
}