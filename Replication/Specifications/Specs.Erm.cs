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
                public static FindSpecification<Account> Accounts()
                {
                    return new FindSpecification<Account>(x => x.IsActive && !x.IsDeleted);
                }

                public static FindSpecification<Category> Categories()
                {
                    return new FindSpecification<Category>(x => x.IsActive && !x.IsDeleted);
                }

                public static FindSpecification<CategoryGroup> CategoryGroups()
                {
                    return new FindSpecification<CategoryGroup>(x => x.IsActive && !x.IsDeleted);
                }

                public static FindSpecification<BranchOfficeOrganizationUnit> BranchOfficeOrganizationUnits()
                {
                    return new FindSpecification<BranchOfficeOrganizationUnit>(x => x.IsActive && !x.IsDeleted);
                }

                public static FindSpecification<CategoryFirmAddress> CategoryFirmAddresses()
                {
                    return new FindSpecification<CategoryFirmAddress>(x => x.IsActive && !x.IsDeleted);
                }

                public static FindSpecification<CategoryOrganizationUnit> CategoryOrganizationUnits()
                {
                    return new FindSpecification<CategoryOrganizationUnit>(x => x.IsActive && !x.IsDeleted);
                }

                public static FindSpecification<Firm> Firms()
                {
                    return new FindSpecification<Firm>(x => x.IsActive && !x.IsDeleted && !x.ClosedForAscertainment);
                }

                public static FindSpecification<FirmAddress> FirmAddresses()
                {
                    return new FindSpecification<FirmAddress>(x => x.IsActive && !x.IsDeleted);
                }

                public static FindSpecification<FirmContact> FirmContacts()
                {
                    return new FindSpecification<FirmContact>(x => true);
                }

                public static FindSpecification<LegalPerson> LegalPersons()
                {
                    return new FindSpecification<LegalPerson>(x => x.IsActive && !x.IsDeleted);
                }

                public static FindSpecification<Client> Clients()
                {
                    return new FindSpecification<Client>(x => x.IsActive && !x.IsDeleted);
                }

                public static FindSpecification<Contact> Contacts()
                {
                    return new FindSpecification<Contact>(x => x.IsActive && !x.IsDeleted);
                }

                public static FindSpecification<Order> Orders()
                {
                    return new FindSpecification<Order>(x => x.IsActive && !x.IsDeleted);
                }
                public static FindSpecification<Project> Projects()
                {
                    return new FindSpecification<Project>(x => x.IsActive);
                }

                public static FindSpecification<Territory> Territories()
                {
                    return new FindSpecification<Territory>(x => x.IsActive);
                }
            }
        }
    }
}