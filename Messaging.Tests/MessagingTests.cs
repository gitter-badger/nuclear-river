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
            var container = new UnityContainer().ConfigureUnity(Resources.UpdateFirm);

            var processorFactory = container.Resolve<IMessageFlowProcessorFactory>();

            var metadataProvider = container.Resolve<IMetadataProvider>();
            var id = "Replicate2AdvancedSearchFlow".AsPrimaryProcessingFlowId();
            MessageFlowMetadata messageFlowMetadata;
            metadataProvider.TryGetMetadata(id, out messageFlowMetadata);

            var settings = container.Resolve<IPerformedOperationsFlowProcessorSettings>();
            var processor = processorFactory.CreateSync(messageFlowMetadata, settings);
            processor.Process();

        }
    }
}
