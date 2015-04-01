using System.IO;
using System.Linq;

using NuClear.AdvancedSearch.ServiceBus.Contracts.DTO;
using NuClear.AdvancedSearch.ServiceBus.Tests.Properties;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.ServiceBus.Tests
{
    [TestFixture]
    public sealed class DeserializationTests
    {
        [Test]
        public void DeserializeTrackedUseCase()
        {
            var stream = new MemoryStream(Resources.UpdateFirm);
            var trackedUseCaseParser = new TrackedUseCaseParser();

            var trackedUseCase = trackedUseCaseParser.Parse(stream);

            Assert.That(trackedUseCase.Operations.Count, Is.EqualTo(1));

            var store = trackedUseCase.Operations.First().ChangesContext.Store;
            Assert.That(store.Count, Is.EqualTo(1));

            var change = store.First();
            Assert.That(change.Key, Is.StringContaining("Firm"));

            Assert.That(change.Value.Count, Is.EqualTo(1));

            var changesDescriptor = change.Value.First().Value;

            Assert.That(changesDescriptor.Id, Is.EqualTo(70000001017863350));
            Assert.That(changesDescriptor.Details.Count, Is.EqualTo(1));

            var changesType = changesDescriptor.Details.First().ChangesType;
            Assert.That(changesType, Is.EqualTo(ChangesType.Updated));
        }
    }
}
