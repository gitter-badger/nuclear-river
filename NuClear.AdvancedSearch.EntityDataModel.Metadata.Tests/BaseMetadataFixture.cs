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
        public string CheckSerialization(IMetadataElement element, string[] propertyNames)
        {
            return Serialize(element, propertyNames);
        }

        protected static TestCaseData Case(MetadataElement element, string json, Metadata properties = Metadata.Identity | Metadata.Features)
        {
            return Case(element, json, properties.ToString().Split(new [] {", "}, StringSplitOptions.RemoveEmptyEntries));
        }

        protected static TestCaseData Case(MetadataElement element, string json, params string[] propertyNames)
        {
            return new TestCaseData(element, propertyNames).Returns(json);
        }

        protected static string Serialize(IMetadataElement element, params string[] propertyNames)
        {
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