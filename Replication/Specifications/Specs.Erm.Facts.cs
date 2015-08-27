using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.Specifications
{
    public static partial class Specs
    {
        public static partial class Erm
        {
            public static class Map
            {
                public static class ToFacts
                {
                    public static MapSpecification<IQuery, IQueryable<Activity>> Activities(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IQueryable<Activity>>(
                            q =>
                            {
                                var appointmentActivities = MapToActivity(q.For(Find.Appointments(ids)), q.For(Find.FirmAppointments()), q.For(Find.ClientAppointments()));
                                var phonecallActivities = MapToActivity(q.For(Find.Phonecalls(ids)), q.For(Find.FirmPhonecalls()), q.For(Find.ClientPhonecalls()));
                                var taskActivities = MapToActivity(q.For(Find.Tasks(ids)), q.For(Find.FirmTasks()), q.For(Find.ClientTasks()));
                                var letterActivities = MapToActivity(q.For(Find.Letters(ids)), q.For(Find.FirmLetters()), q.For(Find.ClientLetters()));

                                return appointmentActivities.Union(phonecallActivities).Union(taskActivities).Union(letterActivities);
                            });
                    }

                    public static MapSpecification<IQuery, IQueryable<Account>> Accounts(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IQueryable<Account>>(
                            q => from account in q.For(Find.Accounts(ids))
                                 select new Account
                                 {
                                     Id = account.Id,
                                     Balance = account.Balance,
                                     BranchOfficeOrganizationUnitId = account.BranchOfficeOrganizationUnitId,
                                     LegalPersonId = account.LegalPersonId,
                                 });
                    }

                    public static MapSpecification<IQuery, IQueryable<BranchOfficeOrganizationUnit>> BranchOfficeOrganizationUnits(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IQueryable<BranchOfficeOrganizationUnit>>(
                            q => from branchOfficeOrganizationUnit in q.For(Find.BranchOfficeOrganizationUnits(ids))
                                 select new BranchOfficeOrganizationUnit
                                 {
                                     Id = branchOfficeOrganizationUnit.Id,
                                     OrganizationUnitId = branchOfficeOrganizationUnit.OrganizationUnitId
                                 });
                    }

                    public static MapSpecification<IQuery, IQueryable<Category>> Categories(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IQueryable<Category>>(
                            q => from category in q.For(Find.Categories(ids))
                                 select new Category
                                 {
                                     Id = category.Id,
                                     Name = category.Name,
                                     Level = category.Level,
                                     ParentId = category.ParentId
                                 });
                    }

                    public static MapSpecification<IQuery, IQueryable<CategoryGroup>> CategoryGroups(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IQueryable<CategoryGroup>>(
                            q => from categoryGroup in q.For(Find.CategoryGroups(ids))
                                 select new CategoryGroup
                                 {
                                     Id = categoryGroup.Id,
                                     Name = categoryGroup.Name,
                                     Rate = categoryGroup.Rate
                                 });
                    }

                    public static MapSpecification<IQuery, IQueryable<CategoryFirmAddress>> CategoryFirmAddresses(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IQueryable<CategoryFirmAddress>>(
                            q => from categoryFirmAddress in q.For(Find.CategoryFirmAddresses(ids))
                                 select new CategoryFirmAddress
                                 {
                                     Id = categoryFirmAddress.Id,
                                     CategoryId = categoryFirmAddress.CategoryId,
                                     FirmAddressId = categoryFirmAddress.FirmAddressId,
                                 });
                    }

                    public static MapSpecification<IQuery, IQueryable<CategoryOrganizationUnit>> CategoryOrganizationUnits(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IQueryable<CategoryOrganizationUnit>>(
                            q => from categoryOrganizationUnit in q.For(Find.CategoryOrganizationUnits(ids))
                                 select new CategoryOrganizationUnit
                                 {
                                     Id = categoryOrganizationUnit.Id,
                                     CategoryId = categoryOrganizationUnit.CategoryId,
                                     CategoryGroupId = categoryOrganizationUnit.CategoryGroupId,
                                     OrganizationUnitId = categoryOrganizationUnit.OrganizationUnitId,
                                 });
                    }

                    public static MapSpecification<IQuery, IQueryable<Client>> Clients(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IQueryable<Client>>(
                            q => from client in q.For(Find.Clients(ids))
                                 select new Client
                                 {
                                     Id = client.Id,
                                     Name = client.Name,
                                     LastDisqualifiedOn = client.LastDisqualifyTime,
                                     HasPhone = (client.MainPhoneNumber ?? client.AdditionalPhoneNumber1 ?? client.AdditionalPhoneNumber2) != null,
                                     HasWebsite = client.Website != null
                                 });
                    }

                    public static MapSpecification<IQuery, IQueryable<Contact>> Contacts(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IQueryable<Contact>>(
                            q => from contact in q.For(Find.Contacts(ids))
                                 select new Contact
                                 {
                                     Id = contact.Id,
                                     Role = ConvertAccountRole(contact.Role),
                                     IsFired = contact.IsFired,
                                     HasPhone = (contact.MainPhoneNumber ?? contact.MobilePhoneNumber ?? contact.HomePhoneNumber ?? contact.AdditionalPhoneNumber) != null,
                                     HasWebsite = contact.Website != null,
                                     ClientId = contact.ClientId
                                 });
                    }

                    public static MapSpecification<IQuery, IQueryable<Firm>> Firms(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IQueryable<Firm>>(
                            q => from firm in q.For(Find.Firms(ids))
                                 select new Firm
                                 {
                                     Id = firm.Id,
                                     Name = firm.Name,
                                     CreatedOn = firm.CreatedOn,
                                     LastDisqualifiedOn = firm.LastDisqualifyTime,
                                     ClientId = firm.ClientId,
                                     OrganizationUnitId = firm.OrganizationUnitId,
                                     OwnerId = firm.OwnerId,
                                     TerritoryId = firm.TerritoryId
                                 });
                    }

                    public static MapSpecification<IQuery, IQueryable<FirmAddress>> FirmAddresses(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IQueryable<FirmAddress>>(
                            q => from firmAddress in q.For(Find.FirmAddresses(ids))
                                 select new FirmAddress
                                 {
                                     Id = firmAddress.Id,
                                     FirmId = firmAddress.FirmId,
                                 });
                    }

                    public static MapSpecification<IQuery, IQueryable<FirmContact>> FirmContacts(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IQueryable<FirmContact>>(
                            q => from firmContact in q.For(Find.FirmContacts(ids))
                                 where firmContact.FirmAddressId != null && (firmContact.ContactType == ContactType.Phone || firmContact.ContactType == ContactType.Website)
                                 select new FirmContact
                                 {
                                     Id = firmContact.Id,
                                     HasPhone = firmContact.ContactType == ContactType.Phone,
                                     HasWebsite = firmContact.ContactType == ContactType.Website,
                                     FirmAddressId = firmContact.FirmAddressId.Value,
                                 });
                    }

                    public static MapSpecification<IQuery, IQueryable<LegalPerson>> LegalPersons(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IQueryable<LegalPerson>>(
                            q => from legalPerson in q.For(Find.LegalPersons(ids))
                                 where legalPerson.ClientId != null
                                 select new LegalPerson
                                 {
                                     Id = legalPerson.Id,
                                     ClientId = legalPerson.ClientId.Value,
                                 });
                    }

                    public static MapSpecification<IQuery, IQueryable<Order>> Orders(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IQueryable<Order>>(
                            q => from order in q.For(Find.Orders(ids))
                                 where new[] { OrderState.OnTermination, OrderState.Archive }.Contains(order.WorkflowStepId)
                                 select new Order
                                 {
                                     Id = order.Id,
                                     EndDistributionDateFact = order.EndDistributionDateFact,
                                     FirmId = order.FirmId,
                                 });
                    }

                    public static MapSpecification<IQuery, IQueryable<Project>> Projects(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IQueryable<Project>>(
                            q => from project in q.For(Find.Projects(ids))
                                 where project.OrganizationUnitId != null
                                 select new Project
                                 {
                                     Id = project.Id,
                                     Name = project.Name,
                                     OrganizationUnitId = project.OrganizationUnitId.Value
                                 });
                    }

                    public static MapSpecification<IQuery, IQueryable<Territory>> Territories(IReadOnlyCollection<long> ids)
                    {
                        return new MapSpecification<IQuery, IQueryable<Territory>>(
                            q => from territory in q.For(Find.Territories(ids))
                                 select new Territory
                                 {
                                     Id = territory.Id,
                                     Name = territory.Name,
                                     OrganizationUnitId = territory.OrganizationUnitId
                                 });
                    }

                    private static IQueryable<Activity> MapToActivity<T, TReference>(
                        IQueryable<T> activities,
                        IQueryable<TReference> firmReferences,
                        IQueryable<TReference> clientReferences)
                        where T : CustomerIntelligence.Model.Erm.ActivityBase
                        where TReference : CustomerIntelligence.Model.Erm.ActivityReference
                    {
                        // TODO {all, 19.08.2015}: Используется FirstOrDefault вместо DefaultIdEmpty из-за бага в данных Erm
                        // В Erm есть Activity, у которых более одного клиента/фирмы в RegardingObjects. Вероятно, во время миграции.
                        // UI отображает первого из многих, а какой из них реальный - сейчас уже не выяснить.
                        // Если найдётся кто-нибудь, кто удалит лишние данные из erm - можно заменить на left join.
                        return from activity in activities
                               let firmReference = firmReferences.Where(x => x.ActivityId == activity.Id).Select(x => (long?)x.ReferencedObjectId).FirstOrDefault()
                               let clientReference = clientReferences.Where(x => x.ActivityId == activity.Id).Select(x => (long?)x.ReferencedObjectId).FirstOrDefault()
                               select new Activity
                                      {
                                          Id = activity.Id,
                                          ModifiedOn = activity.ModifiedOn,
                                          FirmId = firmReference,
                                          ClientId = clientReference
                                      };
                    }

                    private static int ConvertAccountRole(int value)
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

                    private static class ContactType
                    {
                        public const int Phone = 1;
                        public const int Website = 4;
                    }

                    private static class OrderState
                    {
                        public const int OnTermination = 4;
                        public const int Archive = 6;
                    }
                }
            }
        }
    }
}