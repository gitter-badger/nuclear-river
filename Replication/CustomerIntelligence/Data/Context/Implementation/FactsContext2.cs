namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation
{
    public static class FactsContext2
    {
/*
        public static IQueryable<Fact.Firm> Firms(IDataContext context)
        {
            var orderStates = new[] { 4, 6 }; // OnTermination and Archive

            var firms = ErmContext.Firms(context);
            var firmAddresses = ErmContext.FirmAddresses(context);
            var clients = ErmContext.Clients(context);
            var orders = ErmContext.Orders(context).Where(x => orderStates.Contains(x.WorkflowStepId));
            var addressContacts = from contact in ErmContext.FirmContacts(context)
                                  group contact by contact.FirmAddressId
                                      into groupByAddress
                                      select new
                                      {
                                          FirmAddressId = groupByAddress.Key,
                                          HasPhone = groupByAddress.Select(x => x.ContactType == 1).Max(),
                                          HasWebsite = groupByAddress.Select(x => x.ContactType == 4).Max()
                                      };
            var firmContacts = from address in firmAddresses
                               join contact in addressContacts on address.Id equals contact.FirmAddressId
                               select new
                               {
                                   address.FirmId,
                                   contact.HasPhone,
                                   contact.HasWebsite
                               }
                                   into newContacts
                                   group newContacts by newContacts.FirmId
                                       into groupByAddress
                                       select new
                                       {
                                           FirmId = groupByAddress.Key,
                                           HasPhone = groupByAddress.Select(x => x.HasPhone).Max(),
                                           HasWebsite = groupByAddress.Select(x => x.HasWebsite).Max()
                                       };

            return from firm in firms
                   join client in clients on firm.ClientId equals client.Id into firmClients
                   from firmClient in firmClients.DefaultIfEmpty()
                   select new Fact.Firm
                          {
                              Id = firm.Id,
                              Name = firm.Name,
                              CreatedOn = firm.CreatedOn,
                              LastDisqualifiedOn = (firmClient != null ? firmClient.LastDisqualifyTime : firm.LastDisqualifyTime),
                              LastDistributedOn = orders.Where(d => d.FirmId == firm.Id).Max(d => d.EndDistributionDateFact),
                              HasPhone = firmContacts.Where(x => x.FirmId == firm.Id).Max(x => x.HasPhone),
                              HasWebsite = firmContacts.Where(x => x.FirmId == firm.Id).Max(x => x.HasWebsite),
                              AddressCount = firmAddresses.Count(x => x.FirmId == firm.Id),
                              ClientId = firm.ClientId,
                              OrganizationUnitId = firm.OrganizationUnitId,
                              TerritoryId = firm.TerritoryId
                          };
            
        }

        public static IQueryable<Fact.FirmAccount> FirmAccounts(IDataContext context)
        {
            return from firm in ErmContext.Firms(context)
                   join legalPerson in ErmContext.LegalPersons(context) on firm.ClientId equals legalPerson.ClientId
                   join account in ErmContext.Accounts(context) on legalPerson.Id equals account.LegalPersonId
                   select new Fact.FirmAccount
                          {
                              AccountId = account.Id,
                              FirmId = firm.Id,
                              Balance = account.Balance
                          };
        }

        public static IQueryable<Fact.FirmCategory> FirmCategories(IDataContext context)
        {
            // TODO: need to resolve links up to level1 and level2
            return (from categoryFirmAddress in ErmContext.CategoryFirmAddresses(context)
                    join firmAddress in ErmContext.FirmAddresses(context) on categoryFirmAddress.FirmAddressId equals firmAddress.Id
                    select new Fact.FirmCategory
                           {
                               FirmId = firmAddress.FirmId,
                               CategoryId = categoryFirmAddress.CategoryId
                           }).Distinct();
        }

        public static IQueryable<Fact.FirmCategoryGroup> FirmCategoryGroups(IDataContext context)
        {
            return (from firm in ErmContext.Firms(context)
                    join firmAddress in ErmContext.FirmAddresses(context) on firm.Id equals firmAddress.FirmId
                    join categoryFirmAddress in ErmContext.CategoryFirmAddresses(context) on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                    join categoryOrganizationUnit in ErmContext.CategoryOrganizationUnits(context) on categoryFirmAddress.CategoryId equals categoryOrganizationUnit.CategoryId
                    where firm.OrganizationUnitId == categoryOrganizationUnit.OrganizationUnitId
                    select new Fact.FirmCategoryGroup
                           {
                               FirmId = firmAddress.FirmId,
                               CategoryGroupId = categoryOrganizationUnit.CategoryGroupId
                           }).Distinct();
        }

        public static IQueryable<Fact.Client> Clients(IDataContext context)
        {
            return from client in ErmContext.Clients(context)
                   select new Fact.Client
                          {
                              Id = client.Id,
                              Name = client.Name,
                              HasPhone = (client.MainPhoneNumber ?? client.AdditionalPhoneNumber1 ?? client.AdditionalPhoneNumber2) != null,
                              HasWebsite = client.Website != null
                          };
        }

        public static IQueryable<Fact.Contact> Contacts(IDataContext context)
        {
            return from contact in ErmContext.Contacts(context)
                   select new Fact.Contact
                          {
                              Id = contact.Id,
                              Role = ConvertAccountRole(contact.Role),
                              IsFired = contact.IsFired,
                              HasPhone = (contact.MainPhoneNumber ?? contact.MobilePhoneNumber ?? contact.HomePhoneNumber ?? contact.AdditionalPhoneNumber) != null,
                              HasWebsite = contact.Website != null,
                              ClientId = contact.ClientId
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
 */
    }
}