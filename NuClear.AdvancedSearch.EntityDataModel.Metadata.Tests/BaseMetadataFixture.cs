using System;
using System.Collections;
using System.Diagnostics;

using NuClear.Metamodeling.Elements;

using NUnit.Framework;

namespace NuClear.EntityDataModel.Tests
{
    internal abstract class BaseMetadataFixture
    {
        public abstract IEnumerable Provider { get; }

        [TestCaseSource("Provider")]
        public string CheckSerialization(IMetadataElement element, Metadata properties)
        {
            return Serialize(element, properties);
        }

        protected static TestCaseData Case(MetadataElement element)
        {
            return Case(element, Metadata.Identity | Metadata.Features);
        }

        protected static TestCaseData Case(MetadataElement element, Metadata properties)
        {
            return new TestCaseData(element, properties);
        }

        protected static string Serialize(IMetadataElement element, Metadata properties)
        {
            var propertyNames = properties.ToString().Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            
            Debug.WriteLine(element.ToJson(true, propertyNames));

            return element.ToJson(propertyNames).Replace("\"", "'");
        }

        [Flags]
        internal enum Metadata
        {
            Identity = 1,
            Elements = 2,
            Features = 4
        }
    }
}