using System;

using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Erm;

using NUnit.Framework;

// ReSharper disable PossibleUnintendedReferenceComparison
namespace NuClear.AdvancedSearch.Replication.Tests.Transformation
{
    using CI = CustomerIntelligence.Model;
    using Facts = CustomerIntelligence.Model.Facts;

    [TestFixture]
    internal partial class FactsTransformationTests
    {
        private const int ActivityStatusCompleted = 2;
        private const int RegardingObjectReference = 1;

        [Test]
        public void ShouldRecalculateFirmsIfActivityUpdated()
        {
            ErmDb.Has(new Appointment { Id = 1, ModifiedOn = DateTimeOffset.Now, IsActive = true, Status = ActivityStatusCompleted })
                 .Has(new AppointmentReference { ActivityId = 1, Reference = RegardingObjectReference, ReferencedObjectId = 2, ReferencedType = EntityTypeIds.Client },
                      new AppointmentReference { ActivityId = 1, Reference = RegardingObjectReference, ReferencedObjectId = 3, ReferencedType = EntityTypeIds.Firm });

            FactsDb.Has(new Facts::Activity { Id = 1, ClientId = 2, FirmId = 3, ModifiedOn = DateTimeOffset.Now });
            FactsDb.Has(new Facts::Client { Id = 2 });
            FactsDb.Has(new Facts::Firm { Id = 3 });
            FactsDb.Has(new Facts::Firm { Id = 4, ClientId = 2 });
            FactsDb.Has(new Facts::Firm { Id = 5 });

            Transformation.Create(Query)
                          .Transform(Fact.Operation<Facts::Activity>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(3),
                                          Aggregate.Recalculate<CI::Firm>(4)));
        }

        [Test]
        public void ShouldRecalculateFirmsIfActivityCreated()
        {
            ErmDb.Has(new Appointment { Id = 1, ModifiedOn = DateTimeOffset.Now, IsActive = true, Status = ActivityStatusCompleted })
                 .Has(new AppointmentReference { ActivityId = 1, Reference = RegardingObjectReference, ReferencedObjectId = 2, ReferencedType = EntityTypeIds.Client },
                      new AppointmentReference { ActivityId = 1, Reference = RegardingObjectReference, ReferencedObjectId = 3, ReferencedType = EntityTypeIds.Firm });

            FactsDb.Has(new Facts::Client { Id = 2 });
            FactsDb.Has(new Facts::Firm { Id = 3 });
            FactsDb.Has(new Facts::Firm { Id = 4, ClientId = 2 });
            FactsDb.Has(new Facts::Firm { Id = 5 });

            Transformation.Create(Query)
                          .Transform(Fact.Operation<Facts::Activity>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(3),
                                          Aggregate.Recalculate<CI::Firm>(4)));
        }
        [Test]
        public void ShouldRecalculateFirmsIfActivityDeleted()
        {
            FactsDb.Has(new Facts::Activity { Id = 1, ClientId = 2, FirmId = 3, ModifiedOn = DateTimeOffset.Now });
            FactsDb.Has(new Facts::Client { Id = 2 });
            FactsDb.Has(new Facts::Firm { Id = 3 });
            FactsDb.Has(new Facts::Firm { Id = 4, ClientId = 2 });
            FactsDb.Has(new Facts::Firm { Id = 5 });

            Transformation.Create(Query)
                          .Transform(Fact.Operation<Facts::Activity>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Firm>(3),
                                          Aggregate.Recalculate<CI::Firm>(4)));
        }
    }
}