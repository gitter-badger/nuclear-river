using System;
using System.Linq;

using Moq;

using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.AdvancedSearch.Replication.Specifications;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Replication.Tests.Transformation
{
    using Erm = CustomerIntelligence.Model.Erm;

    internal partial class ErmMapToFactsSpecsTests
    {
        [Test]
        public void ShouldTransformAppointmentToActivity()
        {
            ShouldTransform<Erm::Appointment, Erm::AppointmentReference>();
        }

        [Test]
        public void ShouldTransformPhonecallToActivity()
        {
            ShouldTransform<Erm::Phonecall, Erm::PhonecallReference>();
        }

        [Test]
        public void ShouldTransformTaskToActivity()
        {
            ShouldTransform<Erm::Task, Erm::TaskReference>();
        }

        [Test]
        public void ShouldTransformLetterToActivity()
        {
            ShouldTransform<Erm::Letter, Erm::LetterReference>();
        }

        private void ShouldTransform<TActivity, TActivityReference>()
            where TActivity : Erm::ActivityBase, new()
            where TActivityReference : Erm::ActivityReference, new()
        {
            const int ActivityStatusCompleted = 2;
            const int RegardingObjectReference = 1;

            const long FirmId = 111;
            const long ClientId = 222;
            var date = DateTimeOffset.Parse("2015-01-01");

            var query = new Mock<IQuery>();
            query.Setup(q => q.For(It.IsAny<FindSpecification<TActivity>>()),
                        new TActivity { Id = 1, ModifiedOn = date, IsActive = true, Status = ActivityStatusCompleted },
                        new TActivity { Id = 2, ModifiedOn = date, IsActive = true, Status = ActivityStatusCompleted },
                        new TActivity { Id = 3, ModifiedOn = date, IsActive = true, Status = ActivityStatusCompleted },
                        new TActivity { Id = 4, ModifiedOn = date, IsActive = true, Status = ActivityStatusCompleted });

            query.Setup(q => q.For(It.IsAny<FindSpecification<TActivityReference>>()),
                        new TActivityReference { ActivityId = 1, Reference = RegardingObjectReference, ReferencedObjectId = ClientId, ReferencedType = EntityTypeIds.Client },
                        new TActivityReference { ActivityId = 1, Reference = RegardingObjectReference, ReferencedObjectId = FirmId, ReferencedType = EntityTypeIds.Firm },
                        new TActivityReference { ActivityId = 2, Reference = RegardingObjectReference, ReferencedObjectId = FirmId, ReferencedType = EntityTypeIds.Firm },
                        new TActivityReference { ActivityId = 3, Reference = RegardingObjectReference, ReferencedObjectId = ClientId, ReferencedType = EntityTypeIds.Client });

            Assert.That(Specs.Erm.Map.ToFacts.Activities(new[] { 1L }).Map(query.Object).Count(), Is.EqualTo(1));
            Assert.That(Specs.Erm.Map.ToFacts.Activities(new[] { 1L }).Map(query.Object).Single().ClientId, Is.EqualTo(ClientId));
            Assert.That(Specs.Erm.Map.ToFacts.Activities(new[] { 1L }).Map(query.Object).Single().FirmId, Is.EqualTo(FirmId));
            Assert.That(Specs.Erm.Map.ToFacts.Activities(new[] { 1L }).Map(query.Object).Single().ModifiedOn, Is.EqualTo(date));

            Assert.That(Specs.Erm.Map.ToFacts.Activities(new[] { 2L }).Map(query.Object).Count(), Is.EqualTo(1));
            Assert.That(Specs.Erm.Map.ToFacts.Activities(new[] { 2L }).Map(query.Object).Single().ClientId, Is.Null);
            Assert.That(Specs.Erm.Map.ToFacts.Activities(new[] { 2L }).Map(query.Object).Single().FirmId, Is.EqualTo(FirmId));
            Assert.That(Specs.Erm.Map.ToFacts.Activities(new[] { 1L }).Map(query.Object).Single().ModifiedOn, Is.EqualTo(date));

            Assert.That(Specs.Erm.Map.ToFacts.Activities(new[] { 3L }).Map(query.Object).Count(), Is.EqualTo(1));
            Assert.That(Specs.Erm.Map.ToFacts.Activities(new[] { 3L }).Map(query.Object).Single().ClientId, Is.EqualTo(ClientId));
            Assert.That(Specs.Erm.Map.ToFacts.Activities(new[] { 3L }).Map(query.Object).Single().FirmId, Is.Null);
            Assert.That(Specs.Erm.Map.ToFacts.Activities(new[] { 1L }).Map(query.Object).Single().ModifiedOn, Is.EqualTo(date));

            Assert.That(Specs.Erm.Map.ToFacts.Activities(new[] { 4L }).Map(query.Object).Count(), Is.EqualTo(1));
            Assert.That(Specs.Erm.Map.ToFacts.Activities(new[] { 4L }).Map(query.Object).Single().ClientId, Is.Null);
            Assert.That(Specs.Erm.Map.ToFacts.Activities(new[] { 4L }).Map(query.Object).Single().FirmId, Is.Null);
            Assert.That(Specs.Erm.Map.ToFacts.Activities(new[] { 1L }).Map(query.Object).Single().ModifiedOn, Is.EqualTo(date));
        }
   }
}