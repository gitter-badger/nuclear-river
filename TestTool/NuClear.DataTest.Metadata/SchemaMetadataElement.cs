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
        private readonly IConnectionStringIdentity _connectionStringIdentity;

        internal SchemaMetadataElement(IConnectionStringIdentity connectionStringIdentity, IEnumerable<IMetadataFeature> features)
            : base(features)
        {
            _identity = Metadata.Id.For<SchemaMetadataIdentity>(connectionStringIdentity.GetType().Name).Build().AsIdentity();
            _connectionStringIdentity = connectionStringIdentity;
        }

        public override IMetadataElementIdentity Identity => _identity;

        public override void ActualizeId(IMetadataElementIdentity actualMetadataElementIdentity)
        {
            _identity = actualMetadataElementIdentity;
        }

        public IConnectionStringIdentity ConnectionStringIdentity => _connectionStringIdentity;

        public MappingSchema Schema => this.Feature<Linq2DbSchemaFeature>().Schema;

        public IReadOnlyCollection<Type> Entities => this.Features<EntityTypeFeature>().Select(x => x.Type).ToArray();
    }
}