using System.Linq;

using NuClear.AdvancedSearch.Messaging.ServiceBus;
using NuClear.AdvancedSearch.Messaging.Tests.Mocks;
using NuClear.AdvancedSearch.Messaging.Tests.Properties;
using NuClear.OperationsTracking.API.Changes;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Messaging.Tests
{
    [TestFixture]
    public sealed class ServiceBusTests
    {
        [Test]
        public void EmptyUseCase()
        {
            // arrange
            var trackedUseCaseParser = new TrackedUseCaseParser();

            // act
            var trackedUseCase = trackedUseCaseParser.Parse(Resources.EmptyUseCase);

            // assert
            Assert.That(trackedUseCase.Operations, Is.Empty);
        }

        [Test]
        public void UpdateFirmUseCase()
        {
            // arrange
            var trackedUseCaseParser = new TrackedUseCaseParser();

            // act
            var trackedUseCase = trackedUseCaseParser.Parse(Resources.UpdateFirm);

            // assert
            Assert.That(trackedUseCase.Operations.Count, Is.EqualTo(1));

            var store = trackedUseCase.Operations.First().ChangesContext.UntypedChanges;
            Assert.That(store.Count, Is.EqualTo(1));

            var change = store.First();
            Assert.That(change.Key, Is.EqualTo(new UnknownEntityType().SetId(146)));

            Assert.That(change.Value.Count, Is.EqualTo(1));

            var changesDescriptor = change.Value.First().Value;

            Assert.That(changesDescriptor.Id, Is.EqualTo(13));
            Assert.That(changesDescriptor.Details.Count, Is.EqualTo(1));

            var changesType = changesDescriptor.Details.First().ChangesType;
            Assert.That(changesType, Is.EqualTo(ChangesType.Updated));
        }

        [Test]
        public void ComplexUseCase()
        {
            // arrange
            var trackedUseCaseParser = new TrackedUseCaseParser();

            // act
            var trackedUseCase = trackedUseCaseParser.Parse(Resources.ComplexUseCase);

            // assert
            Assert.That(trackedUseCase.Operations.Count, Is.EqualTo(1));
            var store = trackedUseCase.Operations.First().ChangesContext.UntypedChanges;
            Assert.That(store.Count, Is.EqualTo(3));

            var firmChanges = store.Where(x => x.Key.Id == 146).Select(x => x.Value).Single();
            Assert.That(firmChanges.Count, Is.EqualTo(3));

            var firm13Changes = firmChanges.Where(x => x.Key == 13).Select(x => x.Value).Single();
            var firm13ChangesTypes = firm13Changes.Details.Select(x => x.ChangesType);

            Assert.That(firm13ChangesTypes, Contains.Item(ChangesType.Added));
            Assert.That(firm13ChangesTypes, Contains.Item(ChangesType.Updated));
            Assert.That(firm13ChangesTypes, Contains.Item(ChangesType.Deleted));
        }
    }
}
