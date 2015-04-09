using System.Collections.Generic;

using Microsoft.Practices.Unity;

using NuClear.AdvancedSearch.ServiceBus.Tests.DI;
using NuClear.Messaging.API;
using NuClear.Messaging.API.Processing.Processors.Topologies;
using NuClear.OperationsTracking.API.UseCases;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.ServiceBus.Tests
{
    [TestFixture]
    public sealed class MessagingTests
    {
        [Test]
        public void Test1()
        {
            var container = new UnityContainer().ConfigureUnity();
            var topology = container.Resolve<IMessageProcessingTopology>();

            var messages = GetMessage1();
            topology.ProcessAsync(messages);
        }

        private static IReadOnlyList<IMessage> GetMessage1()
        {
            return null;
        }
    }
}
