using System;
using System.Collections.Generic;
using System.Linq;

using Moq;

using NuClear.CustomerIntelligence.Domain;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Provider;
using NuClear.Replication.Core.API.Facts;
using NuClear.Replication.Core.API.Settings;
using NuClear.Replication.Core.Facts;
using NuClear.Replication.Metadata;
using NuClear.Replication.Metadata.Model;
using NuClear.Replication.Metadata.Operations;
using NuClear.Tracing.API;

using NUnit.Framework;

using Facts = NuClear.CustomerIntelligence.Domain.Model.Facts;
using Erm = NuClear.CustomerIntelligence.Domain.Model.Erm;

namespace NuClear.CustomerIntelligence.Replication.Tests.Transformation
{
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

            var metadataSource = new FactsReplicationMetadataSource();
		    var metadataSet = new MetadataSet(metadataSource.Metadata.ToDictionary(x => x.Key, x => x.Value));
            var matadataProvider = new Mock<IMetadataProvider>();
		    matadataProvider.Setup(x => x.TryGetMetadata<ReplicationMetadataIdentity>(out metadataSet))
		                    .Returns(true);

            var factoryInvocationOrder = new List<Type>();
            var factProcessorFactory = new Mock<IFactProcessorFactory>();
			factProcessorFactory.Setup(x => x.Create(It.IsAny<Type>(), It.IsAny<IMetadataElement>()))
			                    .Callback<Type, IMetadataElement>((type, element) => { factoryInvocationOrder.Add(type); })
			                    .Returns(factProcessor.Object);

		    var transformation = new FactsReplicator(matadataProvider.Object,
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
			transformation.Replicate(inputOperations, new Facts::FactTypePriorityComparer());

			//assert
			Assert.That(factoryInvocationOrder.Count, Is.EqualTo(2));
			Assert.That(factoryInvocationOrder[0], Is.EqualTo(typeof(Facts::Firm)));
			Assert.That(factoryInvocationOrder[1], Is.EqualTo(typeof(Facts::FirmAddress)));
		}
	}
}