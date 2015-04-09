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

    public sealed class FactsTransformation : BaseTransformation
    {
        private static readonly Dictionary<Type, FactInfo> Facts = new Dictionary<Type, FactInfo>
        {
            { typeof(Account), new FactInfo(Query.AccountsById, new DependentAggregateInfo(typeof(CI::Firm), FirmRelation.ByAccount)) },
            { typeof(BranchOfficeOrganizationUnit), new FactInfo(Query.BranchOfficeOrganizationUnitsById, new DependentAggregateInfo(typeof(CI::Firm), FirmRelation.ByBranchOfficeOrganizationUnit)) },
            { typeof(CategoryFirmAddress), new FactInfo(Query.CategoryFirmAddressById, new DependentAggregateInfo(typeof(CI::Firm), FirmRelation.ByCategoryFirmAddress)) },
            { typeof(CategoryOrganizationUnit), new FactInfo(Query.CategoryOrganizationUnitById, new DependentAggregateInfo(typeof(CI::Firm), FirmRelation.ByCategoryOrganizationUnit)) },
            { typeof(Client), new FactInfo(Query.ClientsById, new DependentAggregateInfo(typeof(CI::Firm), FirmRelation.ByClient)) { AggregateType = typeof(CI::Client) } },
            { typeof(Contact), new FactInfo(Query.ContactsById, new DependentAggregateInfo(typeof(CI::Firm), FirmRelation.ByContacts)) },
            { typeof(Firm), new FactInfo(Query.FirmsById, new DependentAggregateInfo(typeof(CI::Client), ClientRelation.ByFirm)) { AggregateType = typeof(CI::Firm) } },
            { typeof(FirmAddress), new FactInfo(Query.FirmAddressesById, new DependentAggregateInfo(typeof(CI::Firm), FirmRelation.ByFirmAddress)) },
            { typeof(FirmContact), new FactInfo(Query.FirmContactsById, new DependentAggregateInfo(typeof(CI::Firm), FirmRelation.ByFirmContacts)) },
            { typeof(LegalPerson), new FactInfo(Query.LegalPersonsById, new DependentAggregateInfo(typeof(CI::Firm), FirmRelation.ByLegalPerson)) },
            { typeof(Order), new FactInfo(Query.OrdersById, new DependentAggregateInfo(typeof(CI::Firm), FirmRelation.ByOrder)) }
        };

        private readonly IFactsContext _source;
        private readonly IFactsContext _target;

        public FactsTransformation(IFactsContext source, IFactsContext target, IDataMapper mapper)
            : base(mapper)
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
        }

        public IEnumerable<AggregateOperation> Transform(IEnumerable<FactOperation> operations)
        {
            var result = Enumerable.Empty<AggregateOperation>();

            foreach (var slice in operations.GroupBy(x => new { Operation = x.GetType(), x.FactType }))
            {
                var operation = slice.Key.Operation;
                var factType = slice.Key.FactType;
                var factIds = slice.Select(x => x.FactId).ToArray();

                FactInfo factInfo;
                if (!Facts.TryGetValue(factType, out factInfo))
                {
                    throw new NotSupportedException(string.Format("The '{0}' fact not supported.", factType));
                }

                if (operation == typeof(CreateFact))
                {
                    result = result.Concat(CreateFact(factInfo, factIds));
                }
                
                if (operation == typeof(UpdateFact))
                {
                    result = result.Concat(UpdateFact(factInfo, factIds));
                }

                if (operation == typeof(DeleteFact))
                {
                    result = result.Concat(DeleteFact(factInfo, factIds));
                }
            }

            return result.Distinct();
        }

        private IEnumerable<AggregateOperation> CreateFact(FactInfo info, long[] ids)
        {
            var result = Enumerable.Empty<AggregateOperation>();

            Insert(info.Query(_source, ids));

            if (info.AggregateType != null)
            {
                JoinWith(ref result, ids, id => new InitializeAggregate(info.AggregateType, id));
            }

            foreach (var aggregateInfo in info.Aggregates)
            {
                var aggregateType = aggregateInfo.AggregateType;
                JoinWith(ref result, aggregateInfo.Query(_target, ids).ToArray(), id => new RecalculateAggregate(aggregateType, id));
            }

            return result;
        }

        private IEnumerable<AggregateOperation> UpdateFact(FactInfo info, long[] ids)
        {
            var result = Enumerable.Empty<AggregateOperation>();

            foreach (var aggregateInfo in info.Aggregates)
            {
                var aggregateType = aggregateInfo.AggregateType;
                JoinWith(ref result, aggregateInfo.Query(_target, ids).ToArray(), id => new RecalculateAggregate(aggregateType, id));
            }

            Update(info.Query(_source, ids));

            if (info.AggregateType != null)
            {
                JoinWith(ref result, ids, id => new RecalculateAggregate(info.AggregateType, id));
            }

            foreach (var aggregateInfo in info.Aggregates)
            {
                var aggregateType = aggregateInfo.AggregateType;
                JoinWith(ref result, aggregateInfo.Query(_target, ids).ToArray(), id => new RecalculateAggregate(aggregateType, id));
            }
            
            return result;
        }

        private IEnumerable<AggregateOperation> DeleteFact(FactInfo info, long[] ids)
        {
            var result = Enumerable.Empty<AggregateOperation>();

            foreach (var aggregateInfo in info.Aggregates)
            {
                var aggregateType = aggregateInfo.AggregateType;
                JoinWith(ref result, aggregateInfo.Query(_target, ids).ToArray(), id => new RecalculateAggregate(aggregateType, id));
            }

            Delete(info.Query(_target, ids));

            if (info.AggregateType != null)
            {
                JoinWith(ref result, ids, id => new DestroyAggregate(info.AggregateType, id));
            }

            return result;
        }

        private static void JoinWith(ref IEnumerable<AggregateOperation> collection, IEnumerable<long> ids, Func<long, AggregateOperation> create)
        {
            collection = collection.Concat(ids.Select(create));
        }

        #region Descriptions

        private class FactInfo
        {
            public FactInfo(Func<IFactsContext, IEnumerable<long>, IQueryable> query, params DependentAggregateInfo[] aggregates)
            {
                Query = query;
                Aggregates = aggregates ?? Enumerable.Empty<DependentAggregateInfo>();
            }

            public Func<IFactsContext, IEnumerable<long>, IQueryable> Query { get; private set; }

            public IEnumerable<DependentAggregateInfo> Aggregates { get; private set; }

            public Type AggregateType { get; set; }
        }

        private class DependentAggregateInfo
        {
            public DependentAggregateInfo(Type aggregateType, Func<IFactsContext, IEnumerable<long>, IEnumerable<long>> query)
            {
                AggregateType = aggregateType;
                Query = query;
            }

            public Type AggregateType { get; private set; }

            public Func<IFactsContext, IEnumerable<long>, IEnumerable<long>> Query { get; private set; }
        }

        #endregion

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

            private static IQueryable<TFact> FilterById<TFact>(IQueryable<TFact> facts, IEnumerable<long> ids) where TFact : IIdentifiableObject
            {
                // TODO {all, 08.04.2015}: только для удаления все надо тянуть в память
                return facts.Where(fact => ids.Contains(fact.Id)).ToArray().AsQueryable();
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
