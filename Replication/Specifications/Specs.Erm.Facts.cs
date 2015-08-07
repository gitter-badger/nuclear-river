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
                    public static MapSpecification<IQuery, IQueryable<Account>> Accounts()
                    {
                        return new MapSpecification<IQuery, IQueryable<Account>>(
                            q => from account in q.For(Find.Accounts())
                                 select new Account
                                 {
                                     Id = account.Id,
                                     Balance = account.Balance,
                                     BranchOfficeOrganizationUnitId = account.BranchOfficeOrganizationUnitId,
                                     LegalPersonId = account.LegalPersonId,
                                 });
                    }

                    public static MapSpecification<IQuery, IQueryable<BranchOfficeOrganizationUnit>> BranchOfficeOrganizationUnits()
                    {
                        return new MapSpecification<IQuery, IQueryable<BranchOfficeOrganizationUnit>>(
                            q => from branchOfficeOrganizationUnit in q.For(Find.BranchOfficeOrganizationUnits())
                                 select new BranchOfficeOrganizationUnit
                                 {
                                     Id = branchOfficeOrganizationUnit.Id,
                                     OrganizationUnitId = branchOfficeOrganizationUnit.OrganizationUnitId
                                 });
                    }

                    public static MapSpecification<IQuery, IQueryable<Category>> Categories()
                    {
                        return new MapSpecification<IQuery, IQueryable<Category>>(
                            q => from category in q.For(Find.Categories())
                                 select new Category
                                 {
                                     Id = category.Id,
                                     Name = category.Name,
                                     Level = category.Level,
                                     ParentId = category.ParentId
                                 });
                    }

                    public static MapSpecification<IQuery, IQueryable<CategoryGroup>> CategoryGroups()
                    {
                        return new MapSpecification<IQuery, IQueryable<CategoryGroup>>(
                            q => from categoryGroup in q.For(Find.CategoryGroups())
                                 select new CategoryGroup
                                 {
                                     Id = categoryGroup.Id,
                                     Name = categoryGroup.Name,
                                     Rate = categoryGroup.Rate
                                 });
                    }

                    public static MapSpecification<IQuery, IQueryable<CategoryFirmAddress>> CategoryFirmAddresses()
                    {
                        return new MapSpecification<IQuery, IQueryable<CategoryFirmAddress>>(
                            q => from categoryFirmAddress in q.For(Find.CategoryFirmAddresses())
                                 select new CategoryFirmAddress
                                 {
                                     Id = categoryFirmAddress.Id,
                                     CategoryId = categoryFirmAddress.CategoryId,
                                     FirmAddressId = categoryFirmAddress.FirmAddressId,
                                 });
                    }

                    public static MapSpecification<IQuery, IQueryable<CategoryOrganizationUnit>> CategoryOrganizationUnits()
                    {
                        return new MapSpecification<IQuery, IQueryable<CategoryOrganizationUnit>>(
                            q => from categoryOrganizationUnit in q.For(Find.CategoryOrganizationUnits())
                                 select new CategoryOrganizationUnit
                                 {
                                     Id = categoryOrganizationUnit.Id,
                                     CategoryId = categoryOrganizationUnit.CategoryId,
                                     CategoryGroupId = categoryOrganizationUnit.CategoryGroupId,
                                     OrganizationUnitId = categoryOrganizationUnit.OrganizationUnitId,
                                 });
                    }

                    public static MapSpecification<IQuery, IQueryable<Client>> Clients()
                    {
                        return new MapSpecification<IQuery, IQueryable<Client>>(
                            q => from client in q.For(Find.Clients())
                                 select new Client
                                 {
                                     Id = client.Id,
                                     Name = client.Name,
                                     LastDisqualifiedOn = client.LastDisqualifyTime,
                                     HasPhone = (client.MainPhoneNumber ?? client.AdditionalPhoneNumber1 ?? client.AdditionalPhoneNumber2) != null,
                                     HasWebsite = client.Website != null
                                 });
                    }

                    public static MapSpecification<IQuery, IQueryable<Contact>> Contacts()
                    {
                        return new MapSpecification<IQuery, IQueryable<Contact>>(
                            q => from contact in q.For(Find.Contacts())
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

                    public static MapSpecification<IQuery, IQueryable<Firm>> Firms()
                    {
                        return new MapSpecification<IQuery, IQueryable<Firm>>(
                            q => from firm in q.For(Find.Firms())
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

                    public static MapSpecification<IQuery, IQueryable<FirmAddress>> FirmAddresses()
                    {
                        return new MapSpecification<IQuery, IQueryable<FirmAddress>>(
                            q => from firmAddress in q.For(Find.FirmAddresses())
                                 select new FirmAddress
                                 {
                                     Id = firmAddress.Id,
                                     FirmId = firmAddress.FirmId,
                                 });
                    }

                    public static MapSpecification<IQuery, IQueryable<FirmContact>> FirmContacts()
                    {
                        return new MapSpecification<IQuery, IQueryable<FirmContact>>(
                            q => from firmContact in q.For(Find.FirmContacts())
                                 where firmContact.FirmAddressId != null && (firmContact.ContactType == ContactType.Phone || firmContact.ContactType == ContactType.Website)
                                 select new FirmContact
                                 {
                                     Id = firmContact.Id,
                                     HasPhone = firmContact.ContactType == ContactType.Phone,
                                     HasWebsite = firmContact.ContactType == ContactType.Website,
                                     FirmAddressId = firmContact.FirmAddressId.Value,
                                 });
                    }

                    public static MapSpecification<IQuery, IQueryable<LegalPerson>> LegalPersons()
                    {
                        return new MapSpecification<IQuery, IQueryable<LegalPerson>>(
                            q => from legalPerson in q.For(Find.LegalPersons())
                                 where legalPerson.ClientId != null
                                 select new LegalPerson
                                 {
                                     Id = legalPerson.Id,
                                     ClientId = legalPerson.ClientId.Value,
                                 });
                    }

                    public static MapSpecification<IQuery, IQueryable<Order>> Orders()
                    {
                        return new MapSpecification<IQuery, IQueryable<Order>>(
                            q => from order in q.For(Find.Orders())
                                 where new[] { OrderState.OnTermination, OrderState.Archive }.Contains(order.WorkflowStepId)
                                 select new Order
                                 {
                                     Id = order.Id,
                                     EndDistributionDateFact = order.EndDistributionDateFact,
                                     FirmId = order.FirmId,
                                 });
                    }

                    public static MapSpecification<IQuery, IQueryable<Project>> Projects()
                    {
                        return new MapSpecification<IQuery, IQueryable<Project>>(
                            q => from project in q.For(Find.Projects())
                                 where project.OrganizationUnitId != null
                                 select new Project
                                 {
                                     Id = project.Id,
                                     Name = project.Name,
                                     OrganizationUnitId = project.OrganizationUnitId.Value
                                 });
                    }

                    public static MapSpecification<IQuery, IQueryable<Territory>> Territories()
                    {
                        return new MapSpecification<IQuery, IQueryable<Territory>>(
                            q => from territory in q.For(Find.Territories())
                                 select new Territory
                                 {
                                     Id = territory.Id,
                                     Name = territory.Name,
                                     OrganizationUnitId = territory.OrganizationUnitId
                                 });
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