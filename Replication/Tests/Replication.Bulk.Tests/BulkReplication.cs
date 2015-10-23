using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Replication.Bulk.Tests
{
	[TestFixture]
	public sealed class BulkReplication
	{
	    public static IEnumerable<TestCaseData> GetFactReplication()
            => Program.GetReplicationActions("-fact").Select(tuple => new TestCaseData(tuple.Item2).SetName(tuple.Item1));

	    public static IEnumerable<TestCaseData> GetCiReplication()
            => Program.GetReplicationActions("-ci").Select(tuple => new TestCaseData(tuple.Item2).SetName(tuple.Item1));

	    public static IEnumerable<TestCaseData> GetStatisticsReplication()
	        => Program.GetReplicationActions("-statistics").Select(tuple => new TestCaseData(tuple.Item2).SetName(tuple.Item1));

        [Ignore("Use for bulk replication")]
        [TestCaseSource(nameof(GetFactReplication))]
        public void FactReplicationTest(Action replicationAction)
        {
            replicationAction.Invoke();
        }

        [Ignore("Use for bulk replication")]
        [TestCaseSource(nameof(GetCiReplication))]
        public void CiReplicationTest(Action replicationAction)
        {
            replicationAction.Invoke();
        }

        [Ignore("Use for bulk replication")]
        [TestCaseSource(nameof(GetStatisticsReplication))]
        public void StatisticsReplicationTest(Action replicationAction)
        {
            replicationAction.Invoke();
        }
    }
}
