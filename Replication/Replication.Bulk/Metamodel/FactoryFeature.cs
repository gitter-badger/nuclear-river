using System;

using LinqToDB.Data;

using NuClear.AdvancedSearch.Replication.Bulk.Processors;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Storage.API.Readings;

namespace NuClear.AdvancedSearch.Replication.Bulk.Metamodel
{
    public class FactoryFeature : IUniqueMetadataFeature
    {
        public FactoryFeature(Func<IMetadataElement, IQuery, DataConnection, IMassProcessor> factory)
        {
            Factory = factory;
        }

        public Func<IMetadataElement, IQuery, DataConnection, IMassProcessor> Factory { get; }
    }
}