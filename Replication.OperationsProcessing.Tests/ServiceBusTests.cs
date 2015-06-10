using System.Linq;

using NuClear.AdvancedSearch.Replication.OperationsProcessing.Tests.Mocks;
using NuClear.AdvancedSearch.Replication.OperationsProcessing.Tests.Properties;
using NuClear.OperationsTracking.API.Changes;
using NuClear.Replication.OperationsProcessing.Transports.ServiceBus;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Replication.OperationsProcessing.Tests
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

            var store = trackedUseCase.Operations.First().AffectedEntities.Changes;
            Assert.That(store.Count, Is.EqualTo(1));

            var change = store.First();
            Assert.That(change.Key, Is.EqualTo(new UnknownEntityType().SetId(146)));

            Assert.That(change.Value.Count, Is.EqualTo(1));

            var changesDetails = change.Value.First().Value;
            Assert.That(changesDetails.Count, Is.EqualTo(1));

            var changesType = changesDetails.First().ChangeKind;
            Assert.That(changesType, Is.EqualTo(ChangeKind.Updated));
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
            var store = trackedUseCase.Operations.First().AffectedEntities.Changes;
            Assert.That(store.Count, Is.EqualTo(3));

            var firmChanges = store.Where(x => x.Key.Id == 146).Select(x => x.Value).Single();
            Assert.That(firmChanges.Count, Is.EqualTo(3));

            var firm13Changes = firmChanges.Where(x => x.Key == 13).Select(x => x.Value).Single();
            var firm13ChangesTypes = firm13Changes.Select(x => x.ChangeKind);

            Assert.That(firm13ChangesTypes, Contains.Item(ChangeKind.Added));
            Assert.That(firm13ChangesTypes, Contains.Item(ChangeKind.Updated));
            Assert.That(firm13ChangesTypes, Contains.Item(ChangeKind.Deleted));
        }
    }
}
