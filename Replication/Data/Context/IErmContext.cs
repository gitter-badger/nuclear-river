using System.Linq;

using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.Data.Context
{
    public interface IErmContext
    {
        IQueryable<Erm.Account> Accounts { get; }

        IQueryable<Erm.CategoryFirmAddress> CategoryFirmAddresses { get; }

        IQueryable<Erm.CategoryOrganizationUnit> CategoryOrganizationUnits { get; }

        IQueryable<Erm.Firm> Firms { get; }

        IQueryable<Erm.FirmAddress> FirmAddresses { get; }

        IQueryable<Erm.FirmContact> FirmContacts { get; }

        IQueryable<Erm.LegalPerson> LegalPersons { get; }

        IQueryable<Erm.Client> Clients { get; }

        IQueryable<Erm.Contact> Contacts { get; }

        IQueryable<Erm.Order> Orders { get; }
    }
}