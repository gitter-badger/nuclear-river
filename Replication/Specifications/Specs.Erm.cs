using System.Collections.Generic;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Erm;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.Specifications
{
    public static partial class Specs
    {
        public static partial class Erm
        {
            public static class Find
            {
                public static FindSpecification<Account> Accounts(IReadOnlyCollection<long> ids)
                {
                    return new FindSpecification<Account>(x => x.IsActive && !x.IsDeleted) && API.Specifications.Specs.Find.ByIds<Account>(ids);
                }

                public static FindSpecification<Category> Categories(IReadOnlyCollection<long> ids)
                {
                    return new FindSpecification<Category>(x => x.IsActive && !x.IsDeleted) && API.Specifications.Specs.Find.ByIds<Category>(ids);
                }

                public static FindSpecification<CategoryGroup> CategoryGroups(IReadOnlyCollection<long> ids)
                {
                    return new FindSpecification<CategoryGroup>(x => x.IsActive && !x.IsDeleted) && API.Specifications.Specs.Find.ByIds<CategoryGroup>(ids);
                }

                public static FindSpecification<BranchOfficeOrganizationUnit> BranchOfficeOrganizationUnits(IReadOnlyCollection<long> ids)
                {
                    return new FindSpecification<BranchOfficeOrganizationUnit>(x => x.IsActive && !x.IsDeleted) && API.Specifications.Specs.Find.ByIds<BranchOfficeOrganizationUnit>(ids);
                }

                public static FindSpecification<CategoryFirmAddress> CategoryFirmAddresses(IReadOnlyCollection<long> ids)
                {
                    return new FindSpecification<CategoryFirmAddress>(x => x.IsActive && !x.IsDeleted) && API.Specifications.Specs.Find.ByIds<CategoryFirmAddress>(ids);
                }

                public static FindSpecification<CategoryOrganizationUnit> CategoryOrganizationUnits(IReadOnlyCollection<long> ids)
                {
                    return new FindSpecification<CategoryOrganizationUnit>(x => x.IsActive && !x.IsDeleted) && API.Specifications.Specs.Find.ByIds<CategoryOrganizationUnit>(ids);
                }

                public static FindSpecification<Firm> Firms(IReadOnlyCollection<long> ids)
                {
                    return new FindSpecification<Firm>(x => x.IsActive && !x.IsDeleted && !x.ClosedForAscertainment) && API.Specifications.Specs.Find.ByIds<Firm>(ids);
                }

                public static FindSpecification<FirmAddress> FirmAddresses(IReadOnlyCollection<long> ids)
                {
                    return new FindSpecification<FirmAddress>(x => x.IsActive && !x.IsDeleted) && API.Specifications.Specs.Find.ByIds<FirmAddress>(ids);
                }

                public static FindSpecification<FirmContact> FirmContacts(IReadOnlyCollection<long> ids)
                {
                    return API.Specifications.Specs.Find.ByIds<FirmContact>(ids);
                }

                public static FindSpecification<LegalPerson> LegalPersons(IReadOnlyCollection<long> ids)
                {
                    return new FindSpecification<LegalPerson>(x => x.IsActive && !x.IsDeleted) && API.Specifications.Specs.Find.ByIds<LegalPerson>(ids);
                }

                public static FindSpecification<Client> Clients(IReadOnlyCollection<long> ids)
                {
                    return new FindSpecification<Client>(x => x.IsActive && !x.IsDeleted) && API.Specifications.Specs.Find.ByIds<Client>(ids);
                }

                public static FindSpecification<Contact> Contacts(IReadOnlyCollection<long> ids)
                {
                    return new FindSpecification<Contact>(x => x.IsActive && !x.IsDeleted) && API.Specifications.Specs.Find.ByIds<Contact>(ids);
                }

                public static FindSpecification<Order> Orders(IReadOnlyCollection<long> ids)
                {
                    return new FindSpecification<Order>(x => x.IsActive && !x.IsDeleted) && API.Specifications.Specs.Find.ByIds<Order>(ids);
                }
                public static FindSpecification<Project> Projects(IReadOnlyCollection<long> ids)
                {
                    return new FindSpecification<Project>(x => x.IsActive) && API.Specifications.Specs.Find.ByIds<Project>(ids);
                }

                public static FindSpecification<Territory> Territories(IReadOnlyCollection<long> ids)
                {
                    return new FindSpecification<Territory>(x => x.IsActive) && API.Specifications.Specs.Find.ByIds<Territory>(ids);
                }
            }
        }
    }
}