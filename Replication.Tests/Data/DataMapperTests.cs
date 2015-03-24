using System.Collections.Generic;
using System.Linq;

using LinqToDB.Data;
using LinqToDB.Mapping;

using NuClear.AdvancedSearch.Replication.Model.CustomerIntelligence;
using NuClear.AdvancedSearch.Replication.Model.Erm;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Replication.Tests.Data
{
    [TestFixture, Explicit]
    internal class DataMapperTests : DataFixtureBase
    {
        [Test]
        public void LoadErmFirms()
        {
            using (var ermDb = ErmConnection)
            using (var factDb = FactConnection)
            {
                var firms = ermDb.GetTable<Erm.Firm>().Where(x => x.IsActive && !x.IsDeleted && !x.ClosedForAscertainment);
                var clients = ermDb.GetTable<Erm.Client>().Where(x => x.IsActive && !x.IsDeleted);

                factDb.Reload(from firm in firms
                              join client in clients on firm.ClientId equals client.Id into firmClients
                              from firmClient in firmClients.DefaultIfEmpty()
                              select new Fact.Firm
                              {
                                  Id = firm.Id,
                                  Name = firm.Name,
                                  CreatedOn = firm.CreatedOn,
                                  LastDisqualifiedOn = (firmClient != null ? firmClient.LastDisqualifyTime : firm.LastDisqualifyTime),
                                  LastDistributedOn = null,
                                  HasPhone = false,
                                  HasWebsite = false,
                                  AddressCount = 0,
                                  ClientId = firm.ClientId,
                                  OrganizationUnitId = firm.OrganizationUnitId,
                                  TerritoryId = firm.TerritoryId,
                              });
            }
        }

        [Test]
        public void LoadErmFirmAccounts()
        {
            using (var ermDb = ErmConnection)
            using (var factDb = FactConnection)
            {
                factDb.Reload(from firm in ermDb.GetTable<Erm.Firm>() 
                              where firm.IsActive && !firm.IsDeleted
                              join lp in ermDb.GetTable<Erm.LegalPerson>() on firm.ClientId equals lp.ClientId
                              where lp.IsActive && !lp.IsDeleted
                              join account in ermDb.GetTable<Erm.Account>() on lp.Id equals account.LegalPersonId
                              where account.IsActive && !account.IsDeleted
                              select new Fact.FirmAccount
                              {
                                  AccountId = account.Id,
                                  FirmId = firm.Id,
                                  Balance = account.Balance,
                              });
            }
        }

        [Test]
        public void LoadErmFirmCategories()
        {
            // TODO: need to resolve links up to level1 and level2
            using (var ermDb = ErmConnection)
            using (var factDb = FactConnection)
            {
                var firmAddresses = ermDb.GetTable<Erm.FirmAddress>().Where(x => x.IsActive && !x.IsDeleted && !x.ClosedForAscertainment);
                var categoryFirmAddresses = ermDb.GetTable<Erm.CategoryFirmAddress>().Where(x => x.IsActive && !x.IsDeleted);

                factDb.Reload((
                    from categoryFirmAddress in categoryFirmAddresses
                    join firmAddress in firmAddresses on categoryFirmAddress.FirmAddressId equals firmAddress.Id
                    select new Fact.FirmCategory
                        {
                            FirmId = firmAddress.FirmId, 
                            CategoryId = categoryFirmAddress.CategoryId,
                        }).Distinct());
            }
        }

        [Test]
        public void LoadErmFirmCategoryGroups()
        {
            using (var ermDb = ErmConnection)
            using (var factDb = FactConnection)
            {
                var firms = ermDb.GetTable<Erm.Firm>().Where(x => x.IsActive && !x.IsDeleted && !x.ClosedForAscertainment);
                var firmAddresses = ermDb.GetTable<Erm.FirmAddress>().Where(x => x.IsActive && !x.IsDeleted && !x.ClosedForAscertainment);
                var categoryFirmAddresses = ermDb.GetTable<Erm.CategoryFirmAddress>().Where(x => x.IsActive && !x.IsDeleted);
                var categoryOrganizationUnits = ermDb.GetTable<Erm.CategoryOrganizationUnit>().Where(x => x.IsActive && !x.IsDeleted);

                factDb.Reload((
                    from firm in firms
                    join firmAddress in firmAddresses on firm.Id equals firmAddress.FirmId
                    join categoryFirmAddress in categoryFirmAddresses on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                    join categoryOrganizationUnit in categoryOrganizationUnits on categoryFirmAddress.CategoryId equals categoryOrganizationUnit.CategoryId 
                    where firm.OrganizationUnitId == categoryOrganizationUnit.OrganizationUnitId
                    select new Fact.FirmCategoryGroup
                           {
                               FirmId = firmAddress.FirmId,
                               CategoryGroupId = categoryOrganizationUnit.CategoryGroupId,
                           }).Distinct());
            }
        }

        [Test]
        public void LoadErmClients()
        {
            using (var ermDb = ErmConnection)
            using (var factDb = FactConnection)
            {
                factDb.Reload(from client in ermDb.GetTable<Erm.Client>()
                              where client.IsActive && !client.IsDeleted
                              select new Fact.Client
                                     {
                                         Id = client.Id,
                                         Name = client.Name,
                                         HasPhone = (client.MainPhoneNumber ?? client.AdditionalPhoneNumber1 ?? client.AdditionalPhoneNumber2) != null,
                                         HasWebsite = client.Website != null,
                                     });
            }
        }

        [Test]
        public void LoadErmContacts()
        {
            using (var ermDb = ErmConnection)
            using (var factDb = FactConnection)
            {
                factDb.Reload(from contact in ermDb.GetTable<Erm.Contact>()
                              where contact.IsActive && !contact.IsDeleted
                              select new Fact.Contact
                              {
                                  Id = contact.Id,
                                  Role = MapAccountRole(contact.Role),
                                  IsFired = contact.IsFired,
                                  HasPhone = (contact.MainPhoneNumber ?? contact.MobilePhoneNumber ?? contact.HomePhoneNumber ?? contact.AdditionalPhoneNumber) != null,
                                  HasWebsite = contact.Website != null,
                                  ClientId = contact.ClientId
                              });
            }
        }

        [Test]
        public void Test()
        {
            using (var ermDb = ErmConnection)
            //using (var factDb = FactConnection)
            {
                var contacts = from contact in ermDb.GetTable<Erm.Contact>()
                               where contact.IsActive && !contact.IsDeleted
                               select new Fact.Contact();

                var active = ermDb.GetTable<Erm.Contact>()
                    .Select(contact => new
                               {
                                   contact,
                                   isActive = contact.IsActive && !contact.IsDeleted
                               })
                    .Select(
                                   @t => new
                                         {
                                             Id = @t.contact.Id,
                                             Contact = @t.isActive ? @t.contact : null
                                         });

                var l = active.ToArray().Length;

                //                factDb.Reload(from contact in ermDb.GetTable<Erm.Contact>()
                //                              where contact.IsActive && !contact.IsDeleted
                //                              select new Fact.Contact
                //                              {
                //                                  Id = contact.Id,
                //                                  Role = MapAccountRole(contact.Role),
                //                                  IsFired = contact.IsFired,
                //                                  HasPhone = (contact.MainPhoneNumber ?? contact.MobilePhoneNumber ?? contact.HomePhoneNumber ?? contact.AdditionalPhoneNumber) != null,
                //                                  HasWebsite = contact.Website != null,
                //                                  ClientId = contact.ClientId
                //                              });
            }
        }

        [Test]
        public void LoadFirms()
        {
            using (var factDb = FactConnection)
            using (var ciDb = CustomerIntelligenceConnection)
            {
                ciDb.Reload(
                    from firm in factDb.GetTable<Fact.Firm>()
                    select new CustomerIntelligence.Firm
                           {
                               Id = firm.Id, 
                               Name = firm.Name,
                               CreatedOn = firm.CreatedOn,
                               LastDisqualifiedOn = firm.LastDisqualifiedOn,
                               LastDistributedOn = firm.LastDistributedOn,
                               HasPhone = firm.HasPhone,
                               HasWebsite = firm.HasWebsite,
                               AddressCount = firm.AddressCount,
                               CategoryGroupId = 0,
                               ClientId = firm.ClientId,
                               OrganizationUnitId = firm.OrganizationUnitId,
                               TerritoryId = firm.TerritoryId,
                           });
            }
        }

        [Test]
        public void LoadFirmAccounts()
        {
            using (var factDb = FactConnection)
            using (var ciDb = CustomerIntelligenceConnection)
            {
                ciDb.Reload(
                    from account in factDb.GetTable<Fact.FirmAccount>()
                    select new CustomerIntelligence.FirmAccount
                           {
                               AccountId = account.AccountId,
                               FirmId = account.FirmId,
                               Balance = account.Balance,
                           });
            }
        }

        [Test]
        public void LoadFirmCategories()
        {
            using (var factDb = FactConnection)
            using (var ciDb = CustomerIntelligenceConnection)
            {
                ciDb.Reload(
                    from firmCategory in factDb.GetTable<Fact.FirmCategory>()
                    select new CustomerIntelligence.FirmCategory
                           {
                               FirmId = firmCategory.FirmId,
                               CategoryId = firmCategory.CategoryId,
                           });
            }
        }

        [Test]
        public void LoadFirmCategoryGroups()
        {
            using (var factDb = FactConnection)
            using (var ciDb = CustomerIntelligenceConnection)
            {
                ciDb.Reload(
                    from firmCategoryGroup in factDb.GetTable<Fact.FirmCategoryGroup>()
                    select new CustomerIntelligence.FirmCategoryGroup
                           {
                               FirmId = firmCategoryGroup.FirmId,
                               CategoryGroupId = firmCategoryGroup.CategoryGroupId,
                           });
            }
        }

        [Test]
        public void LoadClients()
        {
            using (var factDb = FactConnection)
            using (var ciDb = CustomerIntelligenceConnection)
            {
                ciDb.Reload(
                    from client in factDb.GetTable<Fact.Client>()
                    select new CustomerIntelligence.Client
                           {
                               Id = client.Id,
                               Name = client.Name,
                               CategoryGroupId = 0
                           });
            }
        }

        [Test]
        public void LoadContacts()
        {
            using (var factDb = FactConnection)
            using (var ciDb = CustomerIntelligenceConnection)
            {
                ciDb.Reload(
                    from contact in factDb.GetTable<Fact.Contact>()
                    select new CustomerIntelligence.Contact
                           {
                               Id = contact.Id,
                               Role = contact.Role,
                               IsFired = contact.IsFired,
                               ClientId = contact.ClientId
                           });
            }
        }

        private static int MapAccountRole(int value)
        {
            switch (value)
            {
                case 200000:
                    return 1;
                case 200001:
                    return 2;
                case 200002:
                    return 3;
                default:
                    return 0;
            }
        }
    }

    internal static class DataConnectionExtensions
    {
        public static void Reload<T>(this DataConnection db, IEnumerable<T> data)
        {
            db.Truncate<T>();
            //db.BulkCopy(data);
        }
       
        public static void Truncate<T>(this DataConnection connection)
        {
            var annotation = connection.MappingSchema.GetAttributes<TableAttribute>(typeof(T)).FirstOrDefault();
            if (annotation != null)
            {
                connection.Execute(string.Format("truncate table [{0}].[{1}]", annotation.Schema, annotation.Name ?? typeof(T).Name));
            }
        }
    }
}