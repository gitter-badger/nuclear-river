using Microsoft.Practices.Unity;

using NuClear.AdvancedSearch.Messaging.Tests.DI;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Messaging.Tests
{
    [TestFixture]
    public sealed class MessagingTests
    {
        [Test]
        public void Test1()
        {
            var container = new UnityContainer().ConfigureUnity();
        }
    }
}
