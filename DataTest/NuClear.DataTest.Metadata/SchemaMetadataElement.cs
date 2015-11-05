using System;
using System.Collections.Generic;
using System.Linq;

using LinqToDB.Mapping;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.DataTest.Metamodel
{
    public sealed class SchemaMetadataElement : MetadataElement<SchemaMetadataElement, SchemaMetadataElementBuilder>
    {
        internal SchemaMetadataElement(string id, IEnumerable<IMetadataFeature> features)
            : base(features)
        {
            Identity = Metadata.Id.For<SchemaMetadataIdentity>(id).Build().AsIdentity();
            Context = id;
        }

        public string Context { get; }

        public override IMetadataElementIdentity Identity { get; }

        public IConnectionStringIdentity ConnectionStringIdentity
            => this.Feature<ConnectionStringFeature>().ConnectionStringIdentity;

        public MappingSchema Schema
            => this.Feature<Linq2DbSchemaFeature>().Schema;

        public IReadOnlyCollection<Type> Entities
            => this.Features<EntityTypeFeature>().Select(x => x.Type).ToArray();

        public override void ActualizeId(IMetadataElementIdentity actualMetadataElementIdentity)
        {
            throw new NotSupportedException();
        }

        public bool TryGetMasterConnectionString(out IConnectionStringIdentity connectionStringIdentity)
        {
            var feature = this.Features<MasterConnectionStringFeature>().FirstOrDefault();
            if (feature != null)
            {
                connectionStringIdentity = feature.ConnectionStringIdentity;
                return true;
            }

            connectionStringIdentity = null;
            return false;
        }
    }
}