using System.Linq;

using NuClear.AdvancedSearch.Replication.Model.CustomerIntelligence;

namespace NuClear.AdvancedSearch.Replication.Data.Context
{
    public interface IFactsContext
    {
        IQueryable<Fact.Account> Accounts { get; }

        IQueryable<Fact.CategoryFirmAddress> CategoryFirmAddresses { get; }

        IQueryable<Fact.CategoryOrganizationUnit> CategoryOrganizationUnits { get; }

        IQueryable<Fact.Client> Clients { get; }

        IQueryable<Fact.Contact> Contacts { get; }

        IQueryable<Fact.Firm> Firms { get; }

        IQueryable<Fact.FirmAddress> FirmAddresses { get; }

        IQueryable<Fact.FirmContact> FirmContacts { get; }

        IQueryable<Fact.LegalPerson> LegalPersons { get; }

        IQueryable<Fact.Order> Orders { get; }
    }
}