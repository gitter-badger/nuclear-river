using System;
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
        private string _id;

        public SchemaMetadataElementBuilder For(string id)
        {
            _id = id;
            return this;
        }

        public SchemaMetadataElementBuilder HasConnectionString<TConnectionStringIdentity>()
            where TConnectionStringIdentity : IdentityBase<TConnectionStringIdentity>, IConnectionStringIdentity, new()
            => WithFeatures(new ConnectionStringFeature(IdentityBase<TConnectionStringIdentity>.Instance));

        public SchemaMetadataElementBuilder HasSchema(MappingSchema schema)
            => WithFeatures(new Linq2DbSchemaFeature(schema));

        public SchemaMetadataElementBuilder HasEntitiesFromNamespace(string ns)
            => WithFeatures(GetTypesFromNamespace(ns));

        public SchemaMetadataElementBuilder HasEntity<T>()
            => WithFeatures(new EntityTypeFeature(typeof(T)));

        private IMetadataFeature[] GetTypesFromNamespace(string ns)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => !assembly.IsDynamic)
                .SelectMany(assembly => assembly.ExportedTypes)
                .Where(type => type.Namespace == ns)
                .Where(type => type.IsClass && !type.IsAbstract && type.DeclaringType == null)
                .Select(type => new EntityTypeFeature(type))
                .Cast<IMetadataFeature>()
                .ToArray();
        }

        protected override SchemaMetadataElement Create()
        {
            return new SchemaMetadataElement(_id, Features);
        }

        public SchemaMetadataElementBuilder HasMasterConnectionString<TConnectionStringIdentity>()
            where TConnectionStringIdentity : IdentityBase<TConnectionStringIdentity>, IConnectionStringIdentity, new()
        {
            return WithFeatures(new MasterConnectionStringFeature(IdentityBase<TConnectionStringIdentity>.Instance));
        }
    }
}
