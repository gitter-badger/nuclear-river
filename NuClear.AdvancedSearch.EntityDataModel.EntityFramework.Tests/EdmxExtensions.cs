using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Text;
using System.Xml;

namespace EntityDataModel.EntityFramework.Tests
{
    internal static class EdmxExtensions
    {
        [Conditional("DEBUG")]
        public static void Dump(this DbModel model)
        {
            var stringBuilder = new StringBuilder();
            
            using (var xmlWriter = XmlWriter.Create(stringBuilder, new XmlWriterSettings { Indent = true }))
            {
                EdmxWriter.WriteEdmx(model, xmlWriter);
            }

            Debug.WriteLine(stringBuilder.ToString());
        }

        public static void Dump(this EdmModel edmModel)
        {
            var stringBuilder = new StringBuilder();

            using (var xmlWriter = XmlWriter.Create(stringBuilder, new XmlWriterSettings { Indent = true }))
            {
                edmModel.ValidateAndSerializeCsdl(xmlWriter);
            }

            Debug.WriteLine(stringBuilder.ToString());
        }

        public static void ValidateAndSerializeCsdl(this EdmModel model, XmlWriter writer)
        {
            var csdlErrors = SerializeAndGetCsdlErrors(model, writer);


            if (csdlErrors.Count > 0)
            {
                Debug.WriteLine(ToErrorMessage(csdlErrors));
            }
        }

        private static List<DataModelErrorEventArgs> SerializeAndGetCsdlErrors(this EdmModel model, XmlWriter writer)
        {
            List<DataModelErrorEventArgs> validationErrors = new List<DataModelErrorEventArgs>();
            CsdlSerializer csdlSerializer = new CsdlSerializer();
            csdlSerializer.OnError += (EventHandler<DataModelErrorEventArgs>)((s, e) => validationErrors.Add(e));
            csdlSerializer.Serialize(model, writer, (string)null);
            return validationErrors;
        }

        public static string ToErrorMessage(this IEnumerable<DataModelErrorEventArgs> validationErrors)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Validation Errors:");
            stringBuilder.AppendLine();
            foreach (DataModelErrorEventArgs modelErrorEventArgs in validationErrors)
                stringBuilder.AppendFormat("{0}.{1}: {2}", modelErrorEventArgs.Item, modelErrorEventArgs.PropertyName, modelErrorEventArgs.ErrorMessage);
            return stringBuilder.ToString();
        }
    }
}