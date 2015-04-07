using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;
using NuClear.AdvancedSearch.Replication.Data;
using NuClear.AdvancedSearch.Replication.Transforming;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    using CI = CustomerIntelligence.Model;

    public sealed class FactsTransformation : BaseTransformation
    {
        private static readonly Dictionary<Type, FactInfo> Facts =
            new Dictionary<Type, FactInfo>
                {
                    { typeof(Account), new FactInfo((context, ids) => context.Accounts.Where(x => ids.Contains(x.Id))) },
                    { typeof(CategoryFirmAddress), new FactInfo((context, ids) => context.CategoryFirmAddresses.Where(x => ids.Contains(x.Id))) },
                    { typeof(CategoryOrganizationUnit), new FactInfo((context, ids) => context.CategoryOrganizationUnits.Where(x => ids.Contains(x.Id))) },
                    { typeof(Client), new FactInfo((context, ids) => context.Clients.Where(x => ids.Contains(x.Id))) },
                    { typeof(Contact), new FactInfo((context, ids) => context.Contacts.Where(x => ids.Contains(x.Id))) },
                    { typeof(Firm), new FactInfo((context, ids) => context.Firms.Where(x => ids.Contains(x.Id)) )},
                    { typeof(FirmAddress), new FactInfo((context, ids) => context.FirmAddresses.Where(x => ids.Contains(x.Id))) },
                    { typeof(FirmContact), new FactInfo((context, ids) => context.FirmContacts.Where(x => ids.Contains(x.Id))) },
                    { typeof(LegalPerson), new FactInfo((context, ids) => context.LegalPersons.Where(x => ids.Contains(x.Id))) },
                    { typeof(Order), new FactInfo((context, ids) => context.Orders.Where(x => ids.Contains(x.Id))) },
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
            var result = new List<AggregateOperation>();

            foreach (var slice in operations.GroupBy(x => new { Operation = x.GetType(), FactType = x.FactType }))
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
                    InsertFact(factInfo, factIds);
                }
                
                if (operation == typeof(UpdateFact))
                {
                    UpdateFact(factInfo, factIds);
                }

                if (operation == typeof(DeleteFact))
                {
                    DeleteFact(factInfo, factIds);
                }

                //var entities = query(GetOperationContext(operation), entityIds);

                //var oldSet = GetInvolvedFirms(factType, factIds);
                //Load(operation, entities);
                //var newSet = GetInvolvedFirms(factType, factIds);

//                result.AddRange(newSet.Except(oldSet).Select(id => new OperationInfo(Operation.Created, typeof(CI::Firm), id)));
//                result.AddRange(newSet.Intersect(oldSet).Select(id => new OperationInfo(Operation.Updated, typeof(CI::Firm), id)));
//                result.AddRange(oldSet.Except(newSet).Select(id => new OperationInfo(Operation.Deleted, typeof(CI::Firm), id)));
            }

            return result;
        }

        private void InsertFact(FactInfo info, long[] ids)
        {
            Insert(info.Query(_source, ids));
        }

        private void UpdateFact(FactInfo info, long[] ids)
        {
            Update(info.Query(_source, ids));
        }

        private void DeleteFact(FactInfo info, long[] ids)
        {
            Delete(info.Query(_target, ids));
        }

        private HashSet<long> GetInvolvedFirms(Type type, IEnumerable<long> ids)
        {
            var context = new CustomerIntelligenceTransformationContext(new FactsFilteredContext(_target, type, ids));
            var firmIds = context.Firms.Select(x => x.Id);
            return new HashSet<long>(firmIds);
        }

        private class FactInfo
        {
            public FactInfo(Func<IFactsContext, IEnumerable<long>, IQueryable> query)
            {
                Query = query;
            }

            public Func<IFactsContext, IEnumerable<long>, IQueryable> Query { get; private set; }
        }
    }
}
