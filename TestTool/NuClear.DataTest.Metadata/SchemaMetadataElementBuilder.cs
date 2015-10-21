using System;
using System.Collections.Generic;
using System.Linq;

using LinqToDB.Mapping;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Model.Common;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.DataTest.Metamodel
{
    public sealed class SchemaMetadataElementBuilder : MetadataElementBuilder<SchemaMetadataElementBuilder, SchemaMetadataElement>
    {
        private readonly List<IMetadataFeature> _features = new List<IMetadataFeature>();
        private IConnectionStringIdentity _identity;

        protected override SchemaMetadataElement Create()
        {
            return new SchemaMetadataElement(_identity, _features);
        }

        public SchemaMetadataElementBuilder For<TConnectionStringIdentity>()
            where TConnectionStringIdentity : IdentityBase<TConnectionStringIdentity>, IConnectionStringIdentity, new()
        {
            _identity = IdentityBase<TConnectionStringIdentity>.Instance;
            return this;
        }

        public SchemaMetadataElementBuilder HasSchema(MappingSchema schema)
        {
            _features.Add(new Linq2DbSchemaFeature(schema));
            return this;
        }

        public SchemaMetadataElementBuilder HasEntitiesFromNamespace(string ns)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(ass => ass.ExportedTypes)
                .Where(type => type.Namespace == ns)
                .Where(type => type.IsClass && !type.IsAbstract && type.DeclaringType == null)
                .Select(type => new EntityTypeFeature(type))
                .ToArray();

            _features.AddRange(types);
            return this;
        }

        public SchemaMetadataElementBuilder HasEntity<T>()
        {
            _features.Add(new EntityTypeFeature(typeof(T)));
            return this;
        }
    }
}
