using System;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Replication.Bulk.Tests
{
    [TestFixture]
    public sealed class BulkReplication
    {
        [Ignore("Use for bulk replication")]
        [Test]
        public void FactReplicationTest(Action replicationAction)
        {
            Program.Main(new[] { "-fact" });
            replicationAction.Invoke();
        }

        [Ignore("Use for bulk replication")]
        [Test]
        public void CiReplicationTest(Action replicationAction)
        {
            Program.Main(new[] { "-ci" });
        }

        [Ignore("Use for bulk replication")]
        [Test]
        public void StatisticsReplicationTest(Action replicationAction)
        {
            Program.Main(new[] { "-statistics" });
        }
    }
}
