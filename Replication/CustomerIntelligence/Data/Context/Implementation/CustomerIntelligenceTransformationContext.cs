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
                
                var clients = from client in _context.Clients
                              join contact in _context.Contacts on client.Id equals contact.ClientId into clientContacts
                              select new
                                     {
                                         client.Id,
                                         client.LastDisqualifiedOn,
                                         HasPhone = client.HasPhone || clientContacts.Any(x => x.HasPhone), // TODO: consider the sql to optimize it if needed
                                         HasWebsite = client.HasWebsite || clientContacts.Any(x => x.HasWebsite) // TODO: consider the sql to optimize it if needed
                                     };

                return from firm in _context.Firms
                       join client in clients on firm.ClientId equals client.Id into firmClients
                       from firmClient in firmClients.DefaultIfEmpty()
                       select new Firm
                              {
                                  Id = firm.Id,
                                  Name = firm.Name,
                                  CreatedOn = firm.CreatedOn,
                                  LastDisqualifiedOn = (firmClient != null ? firmClient.LastDisqualifiedOn : firm.LastDisqualifiedOn),
                                  LastDistributedOn = orders.Where(o => o.FirmId == firm.Id).Select(d => d.EndDistributionDateFact).Cast<DateTimeOffset?>().Max(),
                                  HasPhone = firm.HasPhone || (firmClient != null && firmClient.HasPhone),
                                  HasWebsite = firm.HasWebsite || (firmClient != null && firmClient.HasWebsite),
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

        public IQueryable<FirmAccount> FirmAccounts
        {
            get
            {
                return from firm in _context.Firms
                       join legalPerson in _context.LegalPersons on firm.ClientId equals legalPerson.ClientId
                       join account in _context.Accounts on legalPerson.Id equals account.LegalPersonId
                       select new FirmAccount
                              {
                                  AccountId = account.Id,
                                  FirmId = firm.Id,
                                  Balance = account.Balance
                              };
            }
        }

        public IQueryable<FirmCategory> FirmCategories
        {
            get
            {
                // TODO: need to resolve links up to level1 and level2
                return (from categoryFirmAddress in _context.CategoryFirmAddresses
                        join firmAddress in _context.FirmAddresses on categoryFirmAddress.FirmAddressId equals firmAddress.Id
                        select new FirmCategory
                               {
                                   FirmId = firmAddress.FirmId,
                                   CategoryId = categoryFirmAddress.CategoryId
                               }).Distinct();
            }
        }

        private static class OrderState
        {
            public const int OnTermination = 4;
            public const int Archive = 6;
        }
    }
}