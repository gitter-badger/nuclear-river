using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using LinqToDB;
using LinqToDB.Data;

using NuClear.CustomerIntelligence.Replication.Tests;
using NuClear.CustomerIntelligence.Replication.Tests.BulkLoading;
using NuClear.DataTest.Metamodel;
using NuClear.DataTest.Metamodel.Dsl;
using NuClear.Metamodeling.Provider;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace CustomerIntelligence.Replication.DataTest.Model
{
    // Временная реализация "на выбос", поскольку настоящий bulk-tool пока не дотупен.
    public sealed class BulkReplication<T> : ITestAction
        where T : class
    {
        private readonly ActMetadataElement _metadata;
        private readonly DataConnectionFactory _connectionFactory;
        private readonly IDictionary<string, SchemaMetadataElement> _schemaMetadata;

        public BulkReplication(ActMetadataElement metadata, IMetadataProvider metadataProvider, DataConnectionFactory connectionFactory)
        {
            _metadata = metadata;
            _connectionFactory = connectionFactory;
            _schemaMetadata = metadataProvider.GetMetadataSet<SchemaMetadataIdentity>().Metadata.Values.Cast<SchemaMetadataElement>().ToDictionary(x => x.Context, x => x);
        }

        public void Act()
        {
            using (var sourceDataConnection = _connectionFactory.CreateConnection(_schemaMetadata[_metadata.Source]))
            using (var targetDataConnection = _connectionFactory.CreateConnection(_schemaMetadata[_metadata.Target]))
            {
                var loader = new Loader(sourceDataConnection, targetDataConnection);
                var testInstance = Activator.CreateInstance(typeof(T), loader);
                foreach (var method in typeof(T).GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(info => info.ReturnType == typeof(void)))
                {
                    method.Invoke(testInstance, new object[0]);
                }
            }
        }

        private class Loader : ILoader
        {
            private readonly DataConnection _source;
            private readonly DataConnection _target;

            public Loader(DataConnection source, DataConnection target)
            {
                _source = source;
                _target = target;
            }

            public void Reload<T1>(Func<IQuery, IEnumerable<T1>> loader) where T1 : class
            {
                var query = new Query(_source);
                _target.BulkCopy(loader(query));
            }

            public void Reload<T1>(Func<IQuery, IEnumerable<T1>> loader, Expression<Func<T1, object>> key) where T1 : class
            {
                ITable<T1> temptable = null;
                var query = new Query(_source);
                try
                {
                    var data = loader(query);
                    var datatable = _target.GetTable<T1>();
                    temptable = _target.CreateTable<T1>($"#{Guid.NewGuid():N}");
                    temptable.BulkCopy(new BulkCopyOptions { BulkCopyTimeout = Settings.SqlBulkCopyTimeout }, data);
                    temptable.Join(datatable, key, key, (x, y) => x).Update(datatable, x => x);
                }
                finally
                {
                    temptable?.DropTable();
                }
            }
        }

        private class Query : IQuery
        {
            private readonly DataConnection _dataConnection;

            public Query(DataConnection dataConnection)
            {
                _dataConnection = dataConnection;
            }

            public IQueryable For(Type objType)
            {
                throw new NotImplementedException();
            }

            public IQueryable<T1> For<T1>() where T1 : class
                => _dataConnection.GetTable<T1>();

            public IQueryable<T1> For<T1>(FindSpecification<T1> findSpecification) where T1 : class
                => _dataConnection.GetTable<T1>().Where(findSpecification);
        }
    }
}