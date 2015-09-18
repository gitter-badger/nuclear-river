using System;
using System.Collections.Generic;

using Moq;

using NuClear.AdvancedSearch.Replication.API;
using NuClear.AdvancedSearch.Replication.API.Operations;
using NuClear.AdvancedSearch.Replication.API.Settings;
using NuClear.AdvancedSearch.Replication.API.Transforming.Facts;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.Tracing.API;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Replication.Tests.Transformation
{
	using Erm = CustomerIntelligence.Model.Erm;
	using Facts = CustomerIntelligence.Model.Facts;

	[TestFixture]
    internal class ErmFactsTransformationFixture : TransformationFixtureBase
	{
		[Test]
		public void ShouldProcessFactAccordingToPriority()
		{
			//arrange
            var factProcessor = new Mock<IFactProcessor>();
			factProcessor.Setup(x => x.ApplyChanges(It.IsAny<IReadOnlyCollection<long>>()))
			             .Returns(new IOperation[0]);

			var factoryInvokationOrder = new List<Type>();
			var factProcessorFactory = new Mock<IFactProcessorFactory>();
			factProcessorFactory.Setup(x => x.Create(It.IsAny<IFactInfo>()))
			                    .Callback<IFactInfo>(factInfo => { factoryInvokationOrder.Add(factInfo.Type); })
			                    .Returns(factProcessor.Object);

			var transformation = new ErmFactsTransformation(new ErmFactsTransformationMetadata(),
			                                                factProcessorFactory.Object,
			                                                Mock.Of<IReplicationSettings>(),
			                                                Mock.Of<ITracer>());

			SourceDb.Has(new Erm::Firm { Id = 2 })
			        .Has(new Erm::FirmAddress { Id = 1, FirmId = 1 }, new Erm::FirmAddress { Id = 2, FirmId = 2 });

			TargetDb.Has(new Facts::Firm { Id = 1 });

			var inputOperations = new[]
			                      {
				                      new FactOperation(typeof(Facts::FirmAddress), 1),
				                      new FactOperation(typeof(Facts::Firm), 2),
				                      new FactOperation(typeof(Facts::FirmAddress), 2),
			                      };

			//act
			transformation.Transform(inputOperations);

			//assert
			Assert.That(factoryInvokationOrder.Count, Is.EqualTo(2));
			Assert.That(factoryInvokationOrder[0], Is.EqualTo(typeof(Facts::Firm)));
			Assert.That(factoryInvokationOrder[1], Is.EqualTo(typeof(Facts::FirmAddress)));
		}
	}
}