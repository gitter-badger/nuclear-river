using System;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation
{
    public class CustomerIntelligenceTransformationContext
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

//        public static IQueryable<Firm> Firms(IDataContext context)
//        {
//            // TODO: update HasPhone/HasWebsite from clients and contacts
//            return from firm in context.GetTable<Fact.Firm>()
//                   select new Firm
//                          {
//                              Id = firm.Id,
//                              Name = firm.Name,
//                              CreatedOn = firm.CreatedOn,
//                              LastDisqualifiedOn = firm.LastDisqualifiedOn,
//                              LastDistributedOn = firm.LastDistributedOn,
//                              HasPhone = firm.HasPhone,
//                              HasWebsite = firm.HasWebsite,
//                              AddressCount = firm.AddressCount,
//                              CategoryGroupId = context.GetTable<Fact.FirmCategoryGroup>().Where(x => x.FirmId == firm.Id).Max(x => x.CategoryGroupId),
//                              ClientId = firm.ClientId,
//                              OrganizationUnitId = firm.OrganizationUnitId,
//                              TerritoryId = firm.TerritoryId
//                          };
//        }

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
//
//        public static IQueryable<Client> Clients(IDataContext context)
//        {
//            var firms = context.GetTable<Fact.Firm>();
//            var firmCategoryGroups = context.GetTable<Fact.FirmCategoryGroup>();
//
//            var byClient = from firmCategoryGroup in firmCategoryGroups
//                           join firm in firms on firmCategoryGroup.FirmId equals firm.Id
//                           group new
//                                 {
//                                     firmCategoryGroup.CategoryGroupId
//                                 } by firm.ClientId
//                           into g
//                           select new
//                                  {
//                                      ClientId = g.Key,
//                                      CategoryGroupId = g.Max(x => x.CategoryGroupId)
//                                  };
//
//            return from client in context.GetTable<Fact.Client>()
//                   join cg in byClient on client.Id equals cg.ClientId
//                   select new Client
//                          {
//                              Id = client.Id,
//                              Name = client.Name,
//                              CategoryGroupId = cg.CategoryGroupId
//                          };
//        }
//
//        public static IQueryable<Contact> Contacts(IDataContext context)
//        {
//            return from contact in context.GetTable<Fact.Contact>()
//                   select new Contact
//                          {
//                              Id = contact.Id,
//                              Role = contact.Role,
//                              IsFired = contact.IsFired,
//                              ClientId = contact.ClientId
//                          };
//        }
    }
}