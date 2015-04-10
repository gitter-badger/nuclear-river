using System;

using Microsoft.Practices.Unity;

using NuClear.AdvancedSearch.Messaging.Tests.DI;
using NuClear.AdvancedSearch.Messaging.Tests.Properties;
using NuClear.Messaging.API.Flows.Metadata;
using NuClear.Messaging.API.Processing.Processors;
using NuClear.Metamodeling.Provider;
using NuClear.OperationsProcessing.API;
using NuClear.OperationsProcessing.API.Metadata;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Messaging.Tests
{
    [TestFixture]
    public sealed class MessagingTests
    {
        [Test]
        public void PrimaryTest1()
        {
            var useCases = new[]
            {
                Resources.UpdateFirm,
                Resources.ComplexUseCase
            };

            var flowId = "Replicate2AdvancedSearchFlow".AsPrimaryProcessingFlowId();

            var container = new UnityContainer().ConfigureUnity(useCases);

            var processorFactory = container.Resolve<IMessageFlowProcessorFactory>();
            var metadata = GetMetadata(container, flowId);

            var settings = container.Resolve<IPerformedOperationsFlowProcessorSettings>();
            var processor = processorFactory.CreateSync(metadata, settings);
            processor.Process();
        }

        private static MessageFlowMetadata GetMetadata(IUnityContainer container, Uri id)
        {
            var metadataProvider = container.Resolve<IMetadataProvider>();

            MessageFlowMetadata messageFlowMetadata;
            if (!metadataProvider.TryGetMetadata(id, out messageFlowMetadata))
            {
                throw new ArgumentException();
            }

            return messageFlowMetadata;
        }
    }
}
