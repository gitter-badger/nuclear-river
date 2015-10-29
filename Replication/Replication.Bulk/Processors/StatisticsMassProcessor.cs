using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using LinqToDB;
using LinqToDB.Data;

using NuClear.AdvancedSearch.Common.Metadata.Elements;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.AdvancedSearch.Replication.Bulk.Processors
{
	public class StatisticsMassProcessor<T> : IMassProcessor 
		where T : class, new()
	{
		private readonly StatisticsRecalculationMetadata<T> _info;
	    private readonly FindSpecification<T> _spec;
	    private readonly DataConnection _target;
	    private readonly IQuery _source;

	    public StatisticsMassProcessor(StatisticsRecalculationMetadata<T> info, IQuery query, DataConnection connection)
		{
			_info = info;
            _source = query;
            _target = connection;
            _spec = new FindSpecification<T>(x => true);
        }

		public void Process()
		{
            try
            {
                var sourceQueryable = _info.MapSpecificationProviderForSource.Invoke(_spec).Map(_source);
                var options = new BulkCopyOptions { BulkCopyTimeout = 300 };

                var temptable = _target.CreateTable<T>($"#{Guid.NewGuid():N}");
                temptable.BulkCopy(options, sourceQueryable);

                var primaryKeys = _target.MappingSchema.GetEntityDescriptor(typeof(T)).Columns.Where(x => x.IsPrimaryKey);
                var keyExpression = CreateMappingExpressionForProperties(primaryKeys.Select(x => x.MemberInfo));

                var datatable = _target.GetTable<T>();
                temptable.Join(datatable, keyExpression, keyExpression, (x, y) => x).Update(datatable, x => x);
            }
            catch (Exception ex)
            {
                throw new Exception($"Can not process entity type {typeof(T).Name}", ex);
            }
        }

        public Expression<Func<T, T>> CreateMappingExpressionForProperties(IEnumerable<MemberInfo> primaryKey)
        {
            // TOuter x => new TOuter { primaryKey = x.primaryKey }

            var parameter = Expression.Parameter(typeof(T));
            var newExpression = Expression.New(typeof(T).GetConstructor(new Type[0]));
            var bindings = primaryKey.Select(property => Expression.Bind(property, Expression.MakeMemberAccess(parameter, property)));
            var init = Expression.MemberInit(newExpression, bindings);

            return Expression.Lambda<Func<T, T>>(init, parameter);
        }
    }
}