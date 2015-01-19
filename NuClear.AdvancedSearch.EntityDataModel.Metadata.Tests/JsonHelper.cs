using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

using NuClear.Metamodeling.Elements;

namespace NuClear.EntityDataModel.Tests
{
    internal static class JsonHelper
    {
        public static string ToJson(this object value, params string[] propertyNames)
        {
            return ToJson(value, false, propertyNames);
        }

        public static string ToJson(this object value, bool indented, params string[] propertyNames)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new ElementContractResolver(propertyNames),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Converters = new JsonConverter[] { new StringEnumConverter() }
            };
            return JsonConvert.SerializeObject(value, indented ? Formatting.Indented : Formatting.None, settings);
        }

        private class ElementContractResolver : DefaultContractResolver
        {
            private static readonly HashSet<string> MainProperties = new HashSet<string>(new[] { "Identity", "Kind", "Elements", "Features" });
            
            private readonly HashSet<string> _properties;

            public ElementContractResolver(params string[] propertyNames)
            {
                _properties = propertyNames.Length > 0
                    ? new HashSet<string>(MainProperties.Intersect(propertyNames))
                    : MainProperties;
            }

            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var property = base.CreateProperty(member, memberSerialization);
                
                if (typeof(IMetadataElement).IsAssignableFrom(property.DeclaringType))
                {
                    property.ShouldSerialize = _ => _properties.Contains(property.PropertyName);
                }

                return property;
            }
        }
    }
}
