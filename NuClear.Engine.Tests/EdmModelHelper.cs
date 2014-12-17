using System.Collections.Generic;
using System.Text;
using System.Xml;

using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;

namespace NuClear.AdvancedSearch.Engine.Tests
{
    internal static class EdmModelHelper
    {
        public static string Dump(this IEdmModel model)
        {
            var sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb, new XmlWriterSettings { Indent = true }))
            {
                IEnumerable<EdmError> errors;
                EdmxWriter.TryWriteEdmx(model, writer, EdmxTarget.OData, out errors);
            }
            return sb.ToString();
        }
    }
}