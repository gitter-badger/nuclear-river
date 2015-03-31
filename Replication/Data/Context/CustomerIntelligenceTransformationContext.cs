using System;
using System.Linq;

using LinqToDB;

using NuClear.AdvancedSearch.Replication.Model.CustomerIntelligence;
using NuClear.AdvancedSearch.Replication.Transforming;

namespace NuClear.AdvancedSearch.Replication.Data.Context
{
    public class CustomerIntelligenceTransformationContext
    {
        private readonly FactsContext _context;

        public CustomerIntelligenceTransformationContext(FactsContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            _context = context;
        }

        public static IQueryable<CustomerIntelligence.Firm> Firms(IDataContext context)
        {
            // TODO: update HasPhone/HasWebsite from clients and contacts
            return from firm in context.GetTable<Fact.Firm>()
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
                              CategoryGroupId = context.GetTable<Fact.FirmCategoryGroup>().Where(x => x.FirmId == firm.Id).Max(x => x.CategoryGroupId),
                              ClientId = firm.ClientId,
                              OrganizationUnitId = firm.OrganizationUnitId,
                              TerritoryId = firm.TerritoryId
                          };
        }

        public IQueryable<CustomerIntelligence.FirmAccount> FirmAccounts
        {
            get
            {
                return from firm in _context.Firms
                       join legalPerson in _context.LegalPersons on firm.ClientId equals legalPerson.ClientId
                       join account in _context.Accounts on legalPerson.Id equals account.LegalPersonId
                       select new CustomerIntelligence.FirmAccount
                              {
                                  AccountId = account.Id,
                                  FirmId = firm.Id,
                                  Balance = account.Balance
                              };
            }
        }

        public static IQueryable<CustomerIntelligence.FirmCategory> FirmCategories(IDataContext context)
        {
            return from firmCategory in context.GetTable<Fact.FirmCategory>()
                   select new CustomerIntelligence.FirmCategory
                          {
                              FirmId = firmCategory.FirmId,
                              CategoryId = firmCategory.CategoryId
                          };
        }

        public static IQueryable<CustomerIntelligence.FirmCategoryGroup> FirmCategoryGroups(IDataContext context)
        {
            return from firmCategoryGroup in context.GetTable<Fact.FirmCategoryGroup>()
                   select new CustomerIntelligence.FirmCategoryGroup
                          {
                              FirmId = firmCategoryGroup.FirmId,
                              CategoryGroupId = firmCategoryGroup.CategoryGroupId
                          };
        }

        public static IQueryable<CustomerIntelligence.Client> Clients(IDataContext context)
        {
            var firms = context.GetTable<Fact.Firm>();
            var firmCategoryGroups = context.GetTable<Fact.FirmCategoryGroup>();

            var byClient = from firmCategoryGroup in firmCategoryGroups
                           join firm in firms on firmCategoryGroup.FirmId equals firm.Id
                           group new
                                 {
                                     firmCategoryGroup.CategoryGroupId
                                 } by firm.ClientId
                           into g
                           select new
                                  {
                                      ClientId = g.Key,
                                      CategoryGroupId = g.Max(x => x.CategoryGroupId)
                                  };

            return from client in context.GetTable<Fact.Client>()
                   join cg in byClient on client.Id equals cg.ClientId
                   select new CustomerIntelligence.Client
                          {
                              Id = client.Id,
                              Name = client.Name,
                              CategoryGroupId = cg.CategoryGroupId
                          };
        }

        public static IQueryable<CustomerIntelligence.Contact> Contacts(IDataContext context)
        {
            return from contact in context.GetTable<Fact.Contact>()
                   select new CustomerIntelligence.Contact
                          {
                              Id = contact.Id,
                              Role = contact.Role,
                              IsFired = contact.IsFired,
                              ClientId = contact.ClientId
                          };
        }
    }
}