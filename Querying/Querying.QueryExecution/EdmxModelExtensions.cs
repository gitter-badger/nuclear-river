using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace NuClear.Querying.QueryExecution
{
    public static class EdmxModelExtensions
    {
        private const string ClrTypeAnnotation = "http://schemas.microsoft.com/ado/2013/11/edm/customannotation:ClrType";

        public static IEnumerable<Type> GetClrTypes(this DbModel model)
        {
            var conceptualModel = model.ConceptualModel;
            var metadataItems = conceptualModel.EntityTypes.Cast<MetadataItem>().Union(conceptualModel.ComplexTypes).Union(conceptualModel.EnumTypes);
            var clrTypes = metadataItems
                .SelectMany(x => x.MetadataProperties)
                .Where(x => x.IsAnnotation && x.Name.Equals(ClrTypeAnnotation, StringComparison.Ordinal))
                .Select(x => (Type)x.Value);

            return clrTypes;
        }
    }
}