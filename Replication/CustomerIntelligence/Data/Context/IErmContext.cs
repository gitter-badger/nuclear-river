using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Erm;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context
{
    public interface IErmContext
    {
        IQueryable<Account> Accounts { get; }

        IQueryable<BranchOfficeOrganizationUnit> BranchOfficeOrganizationUnits { get; }

        IQueryable<CategoryFirmAddress> CategoryFirmAddresses { get; }

        IQueryable<CategoryOrganizationUnit> CategoryOrganizationUnits { get; }

        IQueryable<Firm> Firms { get; }

        IQueryable<FirmAddress> FirmAddresses { get; }

        IQueryable<FirmContact> FirmContacts { get; }

        IQueryable<LegalPerson> LegalPersons { get; }

        IQueryable<Client> Clients { get; }

        IQueryable<Contact> Contacts { get; }

        IQueryable<Order> Orders { get; }
    }
}