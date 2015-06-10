using System;
using System.Linq;

using Microsoft.Practices.Unity;

using NuClear.AdvancedSearch.Replication.OperationsProcessing.Tests.DI;
using NuClear.AdvancedSearch.Replication.OperationsProcessing.Tests.Mocks.Receiver;
using NuClear.AdvancedSearch.Replication.OperationsProcessing.Tests.Properties;
using NuClear.Messaging.API;
using NuClear.Messaging.API.Flows.Metadata;
using NuClear.Messaging.API.Processing.Processors;
using NuClear.Metamodeling.Provider;
using NuClear.OperationsProcessing.API;
using NuClear.OperationsProcessing.API.Metadata;
using NuClear.Replication.OperationsProcessing.Metadata.Flows;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Replication.OperationsProcessing.Tests
{
    [TestFixture]
    public sealed class MessagingTests
    {
        [Test]
        public void PrimaryTest1()
        {
            var succeeded = Enumerable.Empty<IMessage>();
            var failed = Enumerable.Empty<IMessage>();

            var receiver = new MockMessageReceiver(new[]
            {
                Resources.UpdateFirm,
                Resources.ComplexUseCase
            },
            (x, y) =>
            {
                succeeded = x;
                failed = y;
            });

            var container = new UnityContainer().ConfigureUnity(receiver);
            var flowId = typeof(ImportFactsFromErmFlow).Name.AsPrimaryProcessingFlowId();

            var processor = GetProcessor(container, flowId);
            processor.Process();

            Assert.That(succeeded.Count(), Is.EqualTo(2));
            Assert.That(failed.Count(), Is.EqualTo(0));
        }

        private static ISyncMessageFlowProcessor GetProcessor(IUnityContainer container, Uri id)
        {
            var metadataProvider = container.Resolve<IMetadataProvider>();

            MessageFlowMetadata messageFlowMetadata;
            if (!metadataProvider.TryGetMetadata(id, out messageFlowMetadata))
            {
                throw new ArgumentException();
            }

            var settings = container.Resolve<IPerformedOperationsFlowProcessorSettings>();
            var processorFactory = container.Resolve<IMessageFlowProcessorFactory>();
            var processor = processorFactory.CreateSync(messageFlowMetadata, settings);

            return processor;
        }
    }
}
