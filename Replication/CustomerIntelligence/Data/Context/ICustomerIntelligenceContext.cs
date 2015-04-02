using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context
{
    public interface ICustomerIntelligenceContext
    {
        IQueryable<Client> Clients { get; }

        IQueryable<Contact> Contacts { get; }

        IQueryable<Firm> Firms { get; }

        IQueryable<FirmBalance> FirmBalances { get; }

        IQueryable<FirmCategory> FirmCategories { get; }
        
        IQueryable<FirmCategoryGroup> FirmCategoryGroups { get; }
    }
}