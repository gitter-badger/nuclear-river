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
        private IMetadataElementIdentity _identity;

        internal SchemaMetadataElement(string id, IEnumerable<IMetadataFeature> features)
            : base(features)
        {
            _identity = Metadata.Id.For<SchemaMetadataIdentity>(id).Build().AsIdentity();
        }

        public override IMetadataElementIdentity Identity => _identity;

        public override void ActualizeId(IMetadataElementIdentity actualMetadataElementIdentity)
        {
            _identity = actualMetadataElementIdentity;
        }

        public IConnectionStringIdentity ConnectionStringIdentity => this.Feature<ConnectionStringFeature>().ConnectionStringIdentity;

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

        public MappingSchema Schema => this.Feature<Linq2DbSchemaFeature>().Schema;

        public IReadOnlyCollection<Type> Entities => this.Features<EntityTypeFeature>().Select(x => x.Type).ToArray();
    }
}