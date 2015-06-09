using System;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Replication.Tests.Transformation
{
    [TestFixture]
    internal class OperationSortingFixture
    {
        [Test]
        public void ShouldSortAggregationOperationsAccordingPriority()
        {
            var comparer = new AggregateOperationPriorityComparer();
            var data = new AggregateOperation[]
                       {
                           new DestroyAggregate(typeof(object), 0),
                           new InitializeAggregate(typeof(object), 0),
                           new RecalculateAggregate(typeof(object), 0),
                       };

            var sortedData = data.OrderByDescending(x => x.GetType(), comparer).ToArray();
            
            Assert.That(sortedData[0], Is.InstanceOf<InitializeAggregate>());
            Assert.That(sortedData[1], Is.InstanceOf<RecalculateAggregate>());
            Assert.That(sortedData[2], Is.InstanceOf<DestroyAggregate>());
        }

        [Test]
        public void ShouldSortFactTypesAccordingPriority()
        {
            var comparer = new FactTypePriorityComparer();
            var data = new[] { typeof(Client), typeof(Project), typeof(object) };

            var sortedData = data.OrderByDescending(x => x, comparer).ToArray();

            Assert.That(sortedData[0], Is.EqualTo(typeof(Project)));
            Assert.That(sortedData[1], Is.EqualTo(typeof(Client)));
            Assert.That(sortedData[2], Is.EqualTo(typeof(object)));
        }
    }
}