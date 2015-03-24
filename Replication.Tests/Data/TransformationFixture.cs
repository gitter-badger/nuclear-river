using System;

using LinqToDB;
using LinqToDB.Mapping;

using NuClear.AdvancedSearch.Replication.Data;
using NuClear.AdvancedSearch.Replication.Model.CustomerIntelligence;
using NuClear.AdvancedSearch.Replication.Model.Erm;
using NuClear.AdvancedSearch.Replication.Transforming;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Replication.Tests.Data
{
    [TestFixture]
    internal class TransformationFixture : DataFixtureBase
    {
        [Test]
        public void Test()
        {
            var cn = CreateConnection("Sqlite", Schema.Erm);
            Array.ForEach(cn.MappingSchema.GetAttributes<TableAttribute>(typeof(Erm.Contact)), x => x.Schema = null);

            cn.CreateTable<Erm.Contact>();
            //cn.Insert(new Erm.Contact());
            FactConnection.Truncate<Fact.Contact>();

            var t = new ErmToCustomerIntelligenceTransformation(cn, FactConnection);
            t.Transform(new[]
                        {
                            new ChangeDescription(206, 585118330926412801, ChangeKind.Created),
                        });

            Assert.That(Read<Fact.Contact>(FactConnection, 585118330926412801), Is.Not.Null);
        }

        [Test]
        public void ShouldCreateContact()
        {
            const long EntityId = 585118330926412801;

            var t = new ErmToCustomerIntelligenceTransformation(ErmConnection, FactConnection);

            FactConnection.Truncate<Fact.Contact>();

            t.Transform(new[]
                        {
                            new ChangeDescription(206, EntityId, ChangeKind.Created),
                            new ChangeDescription(200, 1, ChangeKind.Updated),
                            new ChangeDescription(206, 1156, ChangeKind.Updated),
                        });

            Assert.That(Read<Fact.Contact>(FactConnection, EntityId), Is.Not.Null);
        }
    }
}