using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Replication.Bulk.Tests
{
	[TestFixture]
	public sealed class BulkReplication
	{
	    public static IEnumerable<TestCaseData> FactReplication
            = Program.GetReplicationActions("-fact").Select(tuple => new TestCaseData(tuple.Item2).SetName(tuple.Item1));

	    public static IEnumerable<TestCaseData> CiReplication
            = Program.GetReplicationActions("-ci").Select(tuple => new TestCaseData(tuple.Item2).SetName(tuple.Item1));

	    public static IEnumerable<TestCaseData> StatisticsReplication
            = Program.GetReplicationActions("-statistics").Select(tuple => new TestCaseData(tuple.Item2).SetName(tuple.Item1));


        [Ignore("Use for bulk replication")]
        [TestCaseSource(nameof(FactReplication), Category = "FactReplication")]
        public void FactReplicationTest(Action replicationAction)
        {
            replicationAction.Invoke();
        }

        [Ignore("Use for bulk replication")]
        [TestCaseSource(nameof(CiReplication), Category = "CiReplication")]
        public void CiReplicationTest(Action replicationAction)
        {
            replicationAction.Invoke();
        }

        [Ignore("Use for bulk replication")]
        [TestCaseSource(nameof(StatisticsReplication), Category = "StatisticsReplication")]
        public void StatisticsReplicationTest(Action replicationAction)
        {
            replicationAction.Invoke();
        }
    }
}
