using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Newtonsoft.Json;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts;

namespace NuClear.AdvancedSearch.Replication.Tests.Data
{
    public static class ErmFactsContextDataExtensions
    {
        public static IErmFactsContext ToErmFactsContext(this string jsonContext)
        {
            return JsonConvert.DeserializeObject<ErmFactsContext>(jsonContext) ?? new ErmFactsContext();
        }

        // ReSharper disable MemberCanBePrivate.Local
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        // ReSharper disable once ClassNeverInstantiated.Local
        [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1502:ElementMustNotBeOnSingleLine", Justification = "Reviewed. Suppression is OK here.")]
        private class ErmFactsContext : IErmFactsContext
        {
            public IEnumerable<Account> Accounts { get; set; }
            public IEnumerable<Category> Categories { get; set; }
            public IEnumerable<CategoryGroup> CategoryGroups { get; set; }
            public IEnumerable<BranchOfficeOrganizationUnit> BranchOfficeOrganizationUnits { get; set; }
            public IEnumerable<CategoryFirmAddress> CategoryFirmAddresses { get; set; }
            public IEnumerable<CategoryOrganizationUnit> CategoryOrganizationUnits { get; set; }
            public IEnumerable<Client> Clients { get; set; }
            public IEnumerable<Contact> Contacts { get; set; }
            public IEnumerable<Firm> Firms { get; set; }
            public IEnumerable<FirmAddress> FirmAddresses { get; set; }
            public IEnumerable<FirmContact> FirmContacts { get; set; }
            public IEnumerable<LegalPerson> LegalPersons { get; set; }
            public IEnumerable<Order> Orders { get; set; }
            public IEnumerable<Project> Projects { get; set; }
            public IEnumerable<Territory> Territories { get; set; }

            IQueryable<Account> IErmFactsContext.Accounts { get { return (Accounts ?? Enumerable.Empty<Account>()).AsQueryable(); } }
            IQueryable<Category> IErmFactsContext.Categories { get { return (Categories ?? Enumerable.Empty<Category>()).AsQueryable(); } }
            IQueryable<CategoryGroup> IErmFactsContext.CategoryGroups { get { return (CategoryGroups ?? Enumerable.Empty<CategoryGroup>()).AsQueryable(); } }
            IQueryable<BranchOfficeOrganizationUnit> IErmFactsContext.BranchOfficeOrganizationUnits { get { return (BranchOfficeOrganizationUnits ?? Enumerable.Empty<BranchOfficeOrganizationUnit>()).AsQueryable(); } }
            IQueryable<CategoryFirmAddress> IErmFactsContext.CategoryFirmAddresses { get { return (CategoryFirmAddresses ?? Enumerable.Empty<CategoryFirmAddress>()).AsQueryable(); } }
            IQueryable<CategoryOrganizationUnit> IErmFactsContext.CategoryOrganizationUnits { get { return (CategoryOrganizationUnits ?? Enumerable.Empty<CategoryOrganizationUnit>()).AsQueryable(); } }
            IQueryable<Client> IErmFactsContext.Clients { get { return (Clients ?? Enumerable.Empty<Client>()).AsQueryable(); } }
            IQueryable<Contact> IErmFactsContext.Contacts { get { return (Contacts ?? Enumerable.Empty<Contact>()).AsQueryable(); } }
            IQueryable<Firm> IErmFactsContext.Firms { get { return (Firms ?? Enumerable.Empty<Firm>()).AsQueryable(); } }
            IQueryable<FirmAddress> IErmFactsContext.FirmAddresses { get { return (FirmAddresses ?? Enumerable.Empty<FirmAddress>()).AsQueryable(); } }
            IQueryable<FirmContact> IErmFactsContext.FirmContacts { get { return (FirmContacts ?? Enumerable.Empty<FirmContact>()).AsQueryable(); } }
            IQueryable<LegalPerson> IErmFactsContext.LegalPersons { get { return (LegalPersons ?? Enumerable.Empty<LegalPerson>()).AsQueryable(); } }
            IQueryable<Order> IErmFactsContext.Orders { get { return (Orders ?? Enumerable.Empty<Order>()).AsQueryable(); } }
            IQueryable<Project> IErmFactsContext.Projects { get { return (Projects ?? Enumerable.Empty<Project>()).AsQueryable(); } }
            IQueryable<Territory> IErmFactsContext.Territories { get { return (Territories ?? Enumerable.Empty<Territory>()).AsQueryable(); } }
        }
        // ReSharper restore MemberCanBePrivate.Local
        // ReSharper restore UnusedAutoPropertyAccessor.Local
    }
}
