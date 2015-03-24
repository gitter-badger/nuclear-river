using System;
using System.Collections.Generic;
using System.Linq;

using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Mapping;

using NuClear.AdvancedSearch.Replication.Data;
using NuClear.AdvancedSearch.Replication.Model;
using NuClear.AdvancedSearch.Replication.Model.Erm;
using NuClear.AdvancedSearch.Replication.Model.CustomerIntelligence;

namespace NuClear.AdvancedSearch.Replication.Transforming
{
    public sealed class ErmToCustomerIntelligenceTransformation
    {
        private readonly DataConnection _source;
        private readonly DataConnection _target;
        private readonly Dictionary<int, Type> _map;

        public ErmToCustomerIntelligenceTransformation(DataConnection source, DataConnection target)
        {
            _source = source;
            _target = target;
            _map = new Dictionary<int, Type>
                   {
                       { 142, typeof(Erm.Account) },
                       { 166, typeof(Erm.CategoryFirmAddress) },
                       { 161, typeof(Erm.CategoryOrganizationUnit) },
                       { 200, typeof(Erm.Client) },
                       { 206, typeof(Erm.Contact) },
                       { 146, typeof(Erm.Firm) },
                       { 164, typeof(Erm.FirmAddress) },
                       { 147, typeof(Erm.LegalPerson) }
                   };
        }

        public void Transform(IReadOnlyCollection<ChangeDescription> changes)
        {
//            using (var ermDb = ErmConnection)
//            using (var factDb = FactConnection)
            using (var ermDb = _source)
            using (var factDb = _target)
            {
                foreach (var slice in changes.GroupBy(x => new {x.EntityCode, x.ChangeKind}))
                {
                    var entityCode = slice.Key.EntityCode;
                    var operation = slice.Key.ChangeKind;

//                    Action<DataMapper, IEnumerable<long>> action = (_1, _2) => { };
//                    switch (operation)
//                    {
//                        case ChangeKind.Created:
//                            break;
//                        case ChangeKind.Updated:
//                            break;
//                        case ChangeKind.Deleted:
//                            action = (m, id) => m.Delete(id);
//                            break;
//                    }
//
                    var ids = new HashSet<long>(slice.Select(x => x.EntityId));

                    var ermItems = Extract(ermDb, entityCode, ids);



                    var factItems = Transform(ermItems);

                    factDb.BeginTransaction();
                    var mapper = new DataMapper(factDb);
                    //action(mapper, )
                    LoadContact(factDb, factItems);
                    factDb.CommitTransaction();
                }
            }
        }

        private IEnumerable<IEntity> Extract(IDataContext context, int entityCode, IEnumerable<long> ids)
        {
            Type entityType;
            if (_map.TryGetValue(entityCode, out entityType))
            {
                return context.Read(entityType, ids).Cast<IEntity>();
            }
            return Enumerable.Empty<IEntity>();
        }

        private static IEnumerable<Fact.Contact> Transform(IEnumerable<IEntity> source)
        {
            return source.OfType<Erm.Contact>().Select(
                contact => new Fact.Contact
                           {
                               Id = contact.Id,
                               Role = MapAccountRole(contact.Role),
                               IsFired = contact.IsFired,
                               HasPhone = (contact.MainPhoneNumber ?? contact.MobilePhoneNumber ?? contact.HomePhoneNumber ?? contact.AdditionalPhoneNumber) != null,
                               HasWebsite = contact.Website != null,
                               ClientId = contact.ClientId
                           });
        }

        private static void LoadContact<T>(IDataContext context, IEnumerable<T> items)
        {
            var mapper = new DataMapper(context);

            foreach (var item in items)
            {
                mapper.Insert(item);
            }
        }

        private void Split(IEnumerable<long> ids, IEnumerable<IEntity> items)
        {
            
        }

        private static int MapAccountRole(int value)
        {
            switch (value)
            {
                case 200000:
                    return 1;
                case 200001:
                    return 2;
                case 200002:
                    return 3;
                default:
                    return 0;
            }
        }

        private static DataConnection ErmConnection
        {
            get { return CreateConnection("Erm", Schema.Erm); }
        }

        private static DataConnection FactConnection
        {
            get { return CreateConnection("CustomerIntelligence", Schema.Fact); }
        }

        private static DataConnection CreateConnection(string connectionStringName, MappingSchema schema)
        {
            var db = new DataConnection(connectionStringName);

            db.AddMappingSchema(schema);

            return db;
        }
    }
}
