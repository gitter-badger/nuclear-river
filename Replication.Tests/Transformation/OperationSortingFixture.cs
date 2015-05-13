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

            var sortedData = data.OrderByDescending(x => x, comparer).ToArray();
            
            Assert.That(sortedData[0], Is.InstanceOf<InitializeAggregate>());
            Assert.That(sortedData[1], Is.InstanceOf<RecalculateAggregate>());
            Assert.That(sortedData[2], Is.InstanceOf<DestroyAggregate>());
        }

        [Test]
        public void ShouldSortFactOperationsAccordingPriority()
        {
            var comparer = new FactOperationPriorityComparer();
            var data = new FactOperation[]
                       {
                           new CreateFact(typeof(object), 0), 
                           new DeleteFact(typeof(object), 0), 
                           new UpdateFact(typeof(object), 0), 
                       };

            var sortedData = data.OrderByDescending(x => x, comparer).ToArray();

            Assert.That(sortedData[0], Is.InstanceOf<CreateFact>());
            Assert.That(sortedData[1], Is.InstanceOf<UpdateFact>());
            Assert.That(sortedData[2], Is.InstanceOf<DeleteFact>());
        }

        [Test]
        public void ShouldSortFactTypesAccordingPriority()
        {
            var comparer = new FactTypePriorityComparer();
            var data = new[] { typeof(Client), typeof(Firm), typeof(object) };

            var sortedData = data.OrderByDescending(x => x, comparer).ToArray();

            Assert.That(sortedData[0], Is.EqualTo(typeof(Firm)));
            Assert.That(sortedData[1], Is.EqualTo(typeof(Client)));
            Assert.That(sortedData[2], Is.EqualTo(typeof(object)));
        }
    }
}