using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation
{
    public class CustomerIntelligenceTransformationContext : ICustomerIntelligenceContext
    {
        private readonly IFactsContext _context;

        public CustomerIntelligenceTransformationContext(IFactsContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            _context = context;
        }

        public IQueryable<Firm> Firms
        {
            get
            {
                var orderStates = new HashSet<int>
                                  {
                                      OrderState.OnTermination,
                                      OrderState.Archive
                                  };

                var orders = _context.Orders.Where(x => orderStates.Contains(x.WorkflowStepId));
                /*

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
                 */


                // TODO: update HasPhone/HasWebsite from clients and contacts
                return from firm in _context.Firms
                       join client in _context.Clients on firm.ClientId equals client.Id into firmClients
                       from firmClient in firmClients.DefaultIfEmpty()
                       select new Firm
                              {
                                  Id = firm.Id,
                                  Name = firm.Name,
                                  CreatedOn = firm.CreatedOn,
                                  LastDisqualifiedOn = (firmClient != null ? firmClient.LastDisqualifiedOn : firm.LastDisqualifiedOn),
                                  LastDistributedOn = orders.Where(o => o.FirmId == firm.Id).Select(d => d.EndDistributionDateFact).Cast<DateTimeOffset?>().DefaultIfEmpty().Max(),
                                  //HasPhone = firm.HasPhone || (firmClient != null && firmClient.HasPhone),
                                  //HasWebsite = firm.HasWebsite,
                                  AddressCount = _context.FirmAddresses.Count(fa => fa.FirmId == firm.Id),
                                  //CategoryGroupId = context.GetTable<Fact.FirmCategoryGroup>().Where(x => x.FirmId == firm.Id).Max(x => x.CategoryGroupId),
                                  ClientId = firm.ClientId,
                                  OrganizationUnitId = firm.OrganizationUnitId,
                                  TerritoryId = firm.TerritoryId
                              };
            }
        }

//        public IQueryable<FirmAccount> FirmAccounts
//        {
//            get
//            {
//                return from firm in _context.Firms
//                       join legalPerson in _context.LegalPersons on firm.ClientId equals legalPerson.ClientId
//                       join account in _context.Accounts on legalPerson.Id equals account.LegalPersonId
//                       select new FirmAccount
//                              {
//                                  AccountId = account.Id,
//                                  FirmId = firm.Id,
//                                  Balance = account.Balance
//                              };
//            }
//        }

//        public static IQueryable<FirmCategory> FirmCategories(IDataContext context)
//        {
//            return from firmCategory in context.GetTable<Fact.FirmCategory>()
//                   select new FirmCategory
//                          {
//                              FirmId = firmCategory.FirmId,
//                              CategoryId = firmCategory.CategoryId
//                          };
//        }
//
//        public static IQueryable<FirmCategoryGroup> FirmCategoryGroups(IDataContext context)
//        {
//            return from firmCategoryGroup in context.GetTable<Fact.FirmCategoryGroup>()
//                   select new FirmCategoryGroup
//                          {
//                              FirmId = firmCategoryGroup.FirmId,
//                              CategoryGroupId = firmCategoryGroup.CategoryGroupId
//                          };
//        }

        public IQueryable<Client> Clients
        {
            get
            {
//                var firms = _context.Firms;
//                var firmCategoryGroups = context.GetTable<Fact.FirmCategoryGroup>();
//
//                var byClient = from firmCategoryGroup in firmCategoryGroups
//                               join firm in firms on firmCategoryGroup.FirmId equals firm.Id
//                               group new
//                               {
//                                   firmCategoryGroup.CategoryGroupId
//                               } by firm.ClientId
//                                   into g
//                                   select new
//                                   {
//                                       ClientId = g.Key,
//                                       CategoryGroupId = g.Max(x => x.CategoryGroupId)
//                                   };

                return from client in _context.Clients
                       //join cg in byClient on client.Id equals cg.ClientId
                       select new Client
                              {
                                  Id = client.Id,
                                  Name = client.Name,
                                  //CategoryGroupId = cg.CategoryGroupId
                              };
            }
        }

        public IQueryable<Contact> Contacts
        {
            get
            {
                return from contact in _context.Contacts
                       select new Contact
                              {
                                  Id = contact.Id,
                                  Role = contact.Role,
                                  IsFired = contact.IsFired,
                                  ClientId = contact.ClientId
                              };
            }
        }

        private static class OrderState
        {
            public const int OnTermination = 4;
            public const int Archive = 6;
        }
    }
}