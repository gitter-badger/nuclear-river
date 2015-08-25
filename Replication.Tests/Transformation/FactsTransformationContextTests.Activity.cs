using System;
using System.Linq;

using Moq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Replication.Tests.Transformation
{
    using Erm = CustomerIntelligence.Model.Erm;

    internal partial class FactsTransformationContextTests : BaseTransformationFixture
    {
        private const int FirmEntityType = 146;
        private const int ClientEntityType = 200;

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

        private static void ShouldTransform<TActivity, TActivityReference>()
            where TActivity : Erm::ActivityBase, new()
            where TActivityReference : Erm::ActivityReference, new()
        {
            const long FirmId = 111;
            const long ClientId = 222;
            var date = DateTimeOffset.Parse("2015-01-01");
            
            var data = new object[]
                       {
                           new TActivity { Id = 1, ModifiedOn = date },
                           new TActivityReference { ActivityId = 1, ReferencedObjectId = ClientId, ReferencedType = ClientEntityType },
                           new TActivityReference { ActivityId = 1, ReferencedObjectId = FirmId, ReferencedType = FirmEntityType },

                           new TActivity { Id = 2, ModifiedOn = date },
                           new TActivityReference { ActivityId = 2, ReferencedObjectId = FirmId, ReferencedType = FirmEntityType },

                           new TActivity { Id = 3, ModifiedOn = date },
                           new TActivityReference { ActivityId = 3, ReferencedObjectId = ClientId, ReferencedType = ClientEntityType },

                           new TActivity { Id = 4, ModifiedOn = date },
                       };

            var context = new ErmFactsTransformationContext(CreateErmContext(data));

            Assert.That(context.Activities.ById(1).Count(), Is.EqualTo(1));
            Assert.That(context.Activities.ById(1).Single().ClientId, Is.EqualTo(ClientId));
            Assert.That(context.Activities.ById(1).Single().FirmId, Is.EqualTo(FirmId));
            Assert.That(context.Activities.ById(1).Single().ModifiedOn, Is.EqualTo(date));

            Assert.That(context.Activities.ById(2).Count(), Is.EqualTo(1));
            Assert.That(context.Activities.ById(2).Single().ClientId, Is.Null);
            Assert.That(context.Activities.ById(2).Single().FirmId, Is.EqualTo(FirmId));
            Assert.That(context.Activities.ById(1).Single().ModifiedOn, Is.EqualTo(date));

            Assert.That(context.Activities.ById(3).Count(), Is.EqualTo(1));
            Assert.That(context.Activities.ById(3).Single().ClientId, Is.EqualTo(ClientId));
            Assert.That(context.Activities.ById(3).Single().FirmId, Is.Null);
            Assert.That(context.Activities.ById(1).Single().ModifiedOn, Is.EqualTo(date));

            Assert.That(context.Activities.ById(4).Count(), Is.EqualTo(1));
            Assert.That(context.Activities.ById(4).Single().ClientId, Is.Null);
            Assert.That(context.Activities.ById(4).Single().FirmId, Is.Null);
            Assert.That(context.Activities.ById(1).Single().ModifiedOn, Is.EqualTo(date));
        }

        private static IErmContext CreateErmContext(params object[] data)
        {
            var mock = new Mock<IErmContext>();

            mock.SetupGet(x => x.Appointments)
                .Returns(data.OfType<Erm::Appointment>().AsQueryable());
            mock.SetupGet(x => x.Phonecalls)
                .Returns(data.OfType<Erm::Phonecall>().AsQueryable());
            mock.SetupGet(x => x.Tasks)
                .Returns(data.OfType<Erm::Task>().AsQueryable());
            mock.SetupGet(x => x.Letters)
                .Returns(data.OfType<Erm::Letter>().AsQueryable());

            mock.SetupGet(x => x.AppointmentClients)
                .Returns(data.OfType<Erm::AppointmentReference>().Where(r => r.ReferencedType == ClientEntityType).AsQueryable());
            mock.SetupGet(x => x.PhonecallClients)
                .Returns(data.OfType<Erm::PhonecallReference>().Where(r => r.ReferencedType == ClientEntityType).AsQueryable());
            mock.SetupGet(x => x.TaskClients)
                .Returns(data.OfType<Erm::TaskReference>().Where(r => r.ReferencedType == ClientEntityType).AsQueryable());
            mock.SetupGet(x => x.LetterClients)
                .Returns(data.OfType<Erm::LetterReference>().Where(r => r.ReferencedType == ClientEntityType).AsQueryable());

            mock.SetupGet(x => x.AppointmentFirms)
                .Returns(data.OfType<Erm::AppointmentReference>().Where(r => r.ReferencedType == FirmEntityType).AsQueryable());
            mock.SetupGet(x => x.PhonecallFirms)
                .Returns(data.OfType<Erm::PhonecallReference>().Where(r => r.ReferencedType == FirmEntityType).AsQueryable());
            mock.SetupGet(x => x.TaskFirms)
                .Returns(data.OfType<Erm::TaskReference>().Where(r => r.ReferencedType == FirmEntityType).AsQueryable());
            mock.SetupGet(x => x.LetterFirms)
                .Returns(data.OfType<Erm::LetterReference>().Where(r => r.ReferencedType == FirmEntityType).AsQueryable());

            return mock.Object;
        }
   }
}