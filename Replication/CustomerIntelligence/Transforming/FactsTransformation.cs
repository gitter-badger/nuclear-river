using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;
using NuClear.AdvancedSearch.Replication.Data;
using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    using CI = CustomerIntelligence.Model;

    public sealed class FactsTransformation
    {
        private static readonly Dictionary<Type, FactInfo> Facts = new []
        {
            FactInfo.Create<Account>(Query.AccountsById, FactDependencyInfo.Create<CI::Firm>(FirmRelation.ByAccount)),
            FactInfo.Create<BranchOfficeOrganizationUnit>(Query.BranchOfficeOrganizationUnitsById, FactDependencyInfo.Create<CI::Firm>(FirmRelation.ByBranchOfficeOrganizationUnit)),
            FactInfo.Create<Category>(Query.CategoriesById, FactDependencyInfo.Create<CI::Firm>(FirmRelation.ByCategory)),
            FactInfo.Create<CategoryFirmAddress>(Query.CategoryFirmAddressById, FactDependencyInfo.Create<CI::Firm>(FirmRelation.ByCategoryFirmAddress)),
            FactInfo.Create<CategoryOrganizationUnit>(Query.CategoryOrganizationUnitById, FactDependencyInfo.Create<CI::Firm>(FirmRelation.ByCategoryOrganizationUnit)),
            FactInfo.Create<Client>(Query.ClientsById, FactDependencyInfo.Create<CI::Client>(), FactDependencyInfo.Create<CI::Firm>(FirmRelation.ByClient)),
            FactInfo.Create<Contact>(Query.ContactsById, FactDependencyInfo.Create<CI::Firm>(FirmRelation.ByContacts)),
            FactInfo.Create<Firm>(Query.FirmsById, FactDependencyInfo.Create<CI::Firm>(), FactDependencyInfo.Create<CI::Client>(ClientRelation.ByFirm)),
            FactInfo.Create<FirmAddress>(Query.FirmAddressesById, FactDependencyInfo.Create<CI::Firm>(FirmRelation.ByFirmAddress)),
            FactInfo.Create<FirmContact>(Query.FirmContactsById, FactDependencyInfo.Create<CI::Firm>(FirmRelation.ByFirmContacts)),
            FactInfo.Create<LegalPerson>(Query.LegalPersonsById, FactDependencyInfo.Create<CI::Firm>(FirmRelation.ByLegalPerson)),
            FactInfo.Create<Order>(Query.OrdersById, FactDependencyInfo.Create<CI::Firm>(FirmRelation.ByOrder))
        }.ToDictionary(x => x.FactType);

        private readonly IFactsContext _source;
        private readonly IFactsContext _target;
        private readonly IDataMapper _mapper;

        public FactsTransformation(IFactsContext source, IFactsContext target, IDataMapper mapper)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            _source = source;
            _target = target;
            _mapper = mapper;
        }

        public IEnumerable<AggregateOperation> Transform(IEnumerable<FactOperation> operations)
        {
            var result = Enumerable.Empty<AggregateOperation>();

            var slices = operations.GroupBy(operation => new { Operation = operation, operation.FactType })
                                   .OrderByDescending(slice => slice.Key.Operation, new FactOperationPriorityComparer())
                                   .ThenByDescending(slice => slice.Key.FactType, new FactTypePriorityComparer());

            foreach (var slice in slices)
            {
                var operation = slice.Key.Operation;
                var factType = slice.Key.FactType;
                var factIds = slice.Select(x => x.FactId).ToArray();

                FactInfo factInfo;
                if (!Facts.TryGetValue(factType, out factInfo))
                {
                    throw new NotSupportedException(string.Format("The '{0}' fact not supported.", factType));
                }

                if (operation is CreateFact)
                {
                    result = result.Concat(CreateFact(factInfo, factIds));
                }
                
                if (operation is UpdateFact)
                {
                    result = result.Concat(UpdateFact(factInfo, factIds));
                }

                if (operation is DeleteFact)
                {
                    result = result.Concat(DeleteFact(factInfo, factIds));
                }
            }

            return result.Distinct();
        }

        private IEnumerable<AggregateOperation> CreateFact(FactInfo info, long[] ids)
        {
            _mapper.InsertAll(info.Query(_source, ids));

            return ProcessDependencies(info.Aggregates, ids, (dependency, id) =>
                dependency.IsDirectDependency
                ? (AggregateOperation)new InitializeAggregate(dependency.AggregateType, id)
                : (AggregateOperation)new RecalculateAggregate(dependency.AggregateType, id)).ToArray();
        }

        private IEnumerable<AggregateOperation> UpdateFact(FactInfo info, long[] ids)
        {
            IEnumerable<AggregateOperation> result = ProcessDependencies(info.Aggregates.Where(x => !x.IsDirectDependency), ids, 
                                                     (dependency, id) => new RecalculateAggregate(dependency.AggregateType, id)).ToArray();

            _mapper.UpdateAll(info.Query(_source, ids));

            result = result.Concat(ProcessDependencies(info.Aggregates, ids, (dependency, id) => new RecalculateAggregate(dependency.AggregateType, id)).ToArray());

            return result;
        }

        private IEnumerable<AggregateOperation> DeleteFact(FactInfo info, long[] ids)
        {
            var result = ProcessDependencies(info.Aggregates, ids, (dependency, id) =>
                dependency.IsDirectDependency
                ? (AggregateOperation)new DestroyAggregate(dependency.AggregateType, id)
                : (AggregateOperation)new RecalculateAggregate(dependency.AggregateType, id)).ToArray();

            _mapper.DeleteAll(info.Query(_target, ids));

            return result;
        }

        private IEnumerable<AggregateOperation> ProcessDependencies(IEnumerable<FactDependencyInfo> dependencies, long[] ids, Func<FactDependencyInfo, long, AggregateOperation> build)
        {
            return dependencies.SelectMany(info => info.Query(_target, ids).Select(id => build(info, id)));
        }

        #region Queries

        private static class Query
        {
            public static IQueryable<Account> AccountsById(IFactsContext context, IEnumerable<long> ids)
            {
                return FilterById(context.Accounts, ids);
            }

            public static IQueryable<BranchOfficeOrganizationUnit> BranchOfficeOrganizationUnitsById(IFactsContext context, IEnumerable<long> ids)
            {
                return FilterById(context.BranchOfficeOrganizationUnits, ids);
            }

            public static IQueryable<Category> CategoriesById(IFactsContext context, IEnumerable<long> ids)
            {
                return FilterById(context.Categories, ids);
            }

            public static IQueryable<CategoryFirmAddress> CategoryFirmAddressById(IFactsContext context, IEnumerable<long> ids)
            {
                return FilterById(context.CategoryFirmAddresses, ids);
            }

            public static IQueryable<CategoryOrganizationUnit> CategoryOrganizationUnitById(IFactsContext context, IEnumerable<long> ids)
            {
                return FilterById(context.CategoryOrganizationUnits, ids);
            }

            public static IQueryable<Client> ClientsById(IFactsContext context, IEnumerable<long> ids)
            {
                return FilterById(context.Clients, ids);
            }

            public static IQueryable<Contact> ContactsById(IFactsContext context, IEnumerable<long> ids)
            {
                return FilterById(context.Contacts, ids);
            }

            public static IQueryable<Firm> FirmsById(IFactsContext context, IEnumerable<long> ids)
            {
                return FilterById(context.Firms, ids);
            }

            public static IQueryable<FirmAddress> FirmAddressesById(IFactsContext context, IEnumerable<long> ids)
            {
                return FilterById(context.FirmAddresses, ids);
            }

            public static IQueryable<FirmContact> FirmContactsById(IFactsContext context, IEnumerable<long> ids)
            {
                return FilterById(context.FirmContacts, ids);
            }

            public static IQueryable<LegalPerson> LegalPersonsById(IFactsContext context, IEnumerable<long> ids)
            {
                return FilterById(context.LegalPersons, ids);
            }

            public static IQueryable<Order> OrdersById(IFactsContext context, IEnumerable<long> ids)
            {
                return FilterById(context.Orders, ids);
            }

            public static IQueryable<Project> ProjectsById(IFactsContext context, IEnumerable<long> ids)
            {
                return FilterById(context.Projects, ids);
            }

            public static IQueryable<Territory> TerritoriesById(IFactsContext context, IEnumerable<long> ids)
            {
                return FilterById(context.Territories, ids);
            }

            private static IQueryable<TFact> FilterById<TFact>(IQueryable<TFact> facts, IEnumerable<long> ids) where TFact : IIdentifiableObject
            {
                return facts.Where(fact => ids.Contains(fact.Id));
            }
        }

        #endregion

        #region Relations

        private static class ClientRelation
        {
            public static IEnumerable<long> ByFirm(IFactsContext context, IEnumerable<long> ids)
            {
                return from firm in context.Firms
                       where ids.Contains(firm.Id) && firm.ClientId != null
                       select firm.ClientId.Value;
            }            
        }

        private static class FirmRelation
        {
            public static IEnumerable<long> ByAccount(IFactsContext context, IEnumerable<long> ids)
            {
                return from account in context.Accounts.Where(x => ids.Contains(x.Id))
                       join legalPerson in context.LegalPersons on account.LegalPersonId equals legalPerson.Id
                       join client in context.Clients on legalPerson.ClientId equals client.Id
                       join branchOfficeOrganizationUnit in context.BranchOfficeOrganizationUnits on account.BranchOfficeOrganizationUnitId equals branchOfficeOrganizationUnit.Id
                       join firm in context.Firms on branchOfficeOrganizationUnit.OrganizationUnitId equals firm.OrganizationUnitId
                       where firm.ClientId == client.Id
                       select firm.Id;
            }

            public static IEnumerable<long> ByBranchOfficeOrganizationUnit(IFactsContext context, IEnumerable<long> ids)
            {
                return from account in context.Accounts
                       join legalPerson in context.LegalPersons on account.LegalPersonId equals legalPerson.Id
                       join client in context.Clients on legalPerson.ClientId equals client.Id
                       join branchOfficeOrganizationUnit in context.BranchOfficeOrganizationUnits.Where(x => ids.Contains(x.Id)) on account.BranchOfficeOrganizationUnitId equals branchOfficeOrganizationUnit.Id
                       join firm in context.Firms on branchOfficeOrganizationUnit.OrganizationUnitId equals firm.OrganizationUnitId
                       where firm.ClientId == client.Id
                       select firm.Id;
            }

            public static IEnumerable<long> ByCategory(IFactsContext context, IEnumerable<long> ids)
            {
                var categories1 = context.Categories.Where(x => x.Level == 1);
                var categories2 = context.Categories.Where(x => x.Level == 2);
                var categories3 = context.Categories.Where(x => x.Level == 3);

                var level3 = from firmAddress in context.FirmAddresses
                             join categoryFirmAddress in context.CategoryFirmAddresses on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                             join category3 in categories3 on categoryFirmAddress.CategoryId equals category3.Id
                             where ids.Contains(category3.Id)
                             select firmAddress.FirmId;

                var level2 = from firmAddress in context.FirmAddresses
                             join categoryFirmAddress in context.CategoryFirmAddresses on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                             join category3 in categories3 on categoryFirmAddress.CategoryId equals category3.Id
                             join category2 in categories2 on category3.ParentId equals category2.Id
                             where ids.Contains(category2.Id)
                             select firmAddress.FirmId;

                var level1 = from firmAddress in context.FirmAddresses
                             join categoryFirmAddress in context.CategoryFirmAddresses on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                             join category3 in categories3 on categoryFirmAddress.CategoryId equals category3.Id
                             join category2 in categories2 on category3.ParentId equals category2.Id
                             join category1 in categories1 on category2.ParentId equals category1.Id
                             where ids.Contains(category1.Id)
                             select firmAddress.FirmId;

                return level3.Union(level2).Union(level1);
            }

            public static IEnumerable<long> ByCategoryFirmAddress(IFactsContext context, IEnumerable<long> ids)
            {
                return from categoryFirmAddress in context.CategoryFirmAddresses
                       join firmAddress in context.FirmAddresses on categoryFirmAddress.FirmAddressId equals firmAddress.Id
                       where ids.Contains(categoryFirmAddress.Id)
                       select firmAddress.FirmId;
            }

            public static IEnumerable<long> ByCategoryOrganizationUnit(IFactsContext context, IEnumerable<long> ids)
            {
                return (from firm in context.Firms
                        join firmAddress in context.FirmAddresses on firm.Id equals firmAddress.FirmId
                        join categoryFirmAddress in context.CategoryFirmAddresses on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                        join categoryOrganizationUnit in context.CategoryOrganizationUnits.Where(x => ids.Contains(x.Id)) on categoryFirmAddress.CategoryId equals categoryOrganizationUnit.CategoryId
                        where firm.OrganizationUnitId == categoryOrganizationUnit.OrganizationUnitId
                        select firmAddress.FirmId).Distinct();
            }

            public static IEnumerable<long> ByClient(IFactsContext context, IEnumerable<long> ids)
            {
                return from firm in context.Firms
                       where firm.ClientId != null && ids.Contains(firm.ClientId.Value)
                       select firm.Id;
            }

            public static IEnumerable<long> ByContacts(IFactsContext context, IEnumerable<long> ids)
            {
                return from firm in context.Firms
                       join client in context.Clients on firm.ClientId equals client.Id
                       join contact in context.Contacts on client.Id equals contact.ClientId
                       where ids.Contains(contact.Id)
                       select firm.Id;
            }

            public static IEnumerable<long> ByFirmAddress(IFactsContext context, IEnumerable<long> ids)
            {
                return from firmAddress in context.FirmAddresses
                       where ids.Contains(firmAddress.Id)
                       select firmAddress.FirmId;
            }

            public static IEnumerable<long> ByFirmContacts(IFactsContext context, IEnumerable<long> ids)
            {
                return from firmAddress in context.FirmAddresses
                       join firmContact in context.FirmContacts on firmAddress.Id equals firmContact.FirmAddressId
                       where ids.Contains(firmContact.Id)
                       select firmAddress.FirmId;
            }

            public static IEnumerable<long> ByLegalPerson(IFactsContext context, IEnumerable<long> ids)
            {
                return from account in context.Accounts
                       join legalPerson in context.LegalPersons.Where(x => ids.Contains(x.Id)) on account.LegalPersonId equals legalPerson.Id
                       join client in context.Clients on legalPerson.ClientId equals client.Id
                       join branchOfficeOrganizationUnit in context.BranchOfficeOrganizationUnits on account.BranchOfficeOrganizationUnitId equals branchOfficeOrganizationUnit.Id
                       join firm in context.Firms on branchOfficeOrganizationUnit.OrganizationUnitId equals firm.OrganizationUnitId
                       where firm.ClientId == client.Id
                       select firm.Id;
            }

            public static IEnumerable<long> ByOrder(IFactsContext context, IEnumerable<long> ids)
            {
                return from order in context.Orders
                       where ids.Contains(order.Id)
                       select order.FirmId;
            }
        }

        #endregion
    }
}
