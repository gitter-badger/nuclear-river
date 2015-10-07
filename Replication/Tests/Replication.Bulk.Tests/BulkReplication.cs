using System;
using System.Collections.Generic;
using System.Linq;

using LinqToDB.Data;

using NuClear.AdvancedSearch.Replication.Bulk.Processors;
using NuClear.Storage.API.Readings;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Replication.Bulk.Tests
{
	[TestFixture]
	public sealed class BulkReplication
	{
		private static readonly IReadOnlyCollection<TestCaseData> ErmToFacts = Source(Context.ErmToFacts).ToArray();
		private static readonly IReadOnlyCollection<TestCaseData> FactsToCustomerIntelligence = Source(Context.FactsToCustomerIntelligence).ToArray();
		private static readonly IReadOnlyCollection<TestCaseData> FactsToCustomerIntelligenceStatistics = Source(Context.FactsToCustomerIntelligenceStatistics).ToArray();

        [Ignore("Use for bulk replication")]
        [TestCaseSource(nameof(ErmToFacts))]
		[TestCaseSource(nameof(FactsToCustomerIntelligence))]
		[TestCaseSource(nameof(FactsToCustomerIntelligenceStatistics))]
		public void Test<TInfo>(Storage sourceContext, Storage targetContext, Func<TInfo, IQuery, DataConnection, IMassProcessor> factory, TInfo info)
		{
			using (var source = sourceContext.CreateConnection())
			using (var target = targetContext.CreateConnection())
			{
				var sourceQuery = new LinqToDbQuery(source);
				var processor = factory.Invoke(info, sourceQuery, target);
				processor.Process();
			}
		}

		private static IEnumerable<TestCaseData> Source(MassReplicationContext context)
		{
			foreach (var value in context.Metadata)
			{
				var data = new TestCaseData(context.Source, context.Target, context.Factory, value);
				data.SetName($"{context.Source.ConnectionStringName} -> {context.Target.ConnectionStringName}, {value.Identity.Id}");
                yield return data;
			}
		}
	}
}
