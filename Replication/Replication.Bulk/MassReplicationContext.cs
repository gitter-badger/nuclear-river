using System;
using System.Collections.Generic;

using LinqToDB.Data;

using NuClear.AdvancedSearch.Replication.Bulk.Processors;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Concrete.Hierarchy;
using NuClear.Storage.API.Readings;

namespace NuClear.AdvancedSearch.Replication.Bulk
{
	public sealed class MassReplicationContext
	{
		public MassReplicationContext(Storage source, Storage target, HierarchyMetadata metadata, Func<IMetadataElement, IQuery, DataConnection, IMassProcessor> factory, IEnumerable<string> essentialViews)
		{
			Source = source;
			Target = target;
			Metadata = metadata.Elements;
			Factory = factory;
		    EssentialViews = essentialViews;
		}

		public Storage Source { get; }
		public Storage Target { get; }
		public IEnumerable<IMetadataElement> Metadata { get; }
		public Func<IMetadataElement, IQuery, DataConnection, IMassProcessor> Factory { get; }
	    public IEnumerable<string> EssentialViews { get; }
	}
}