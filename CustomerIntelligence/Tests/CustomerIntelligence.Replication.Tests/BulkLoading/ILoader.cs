using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using NuClear.Storage.API.Readings;

namespace NuClear.CustomerIntelligence.Replication.Tests.BulkLoading
{
    public interface ILoader
    {
        void Reload<T>(Func<IQuery, IEnumerable<T>> loader)
            where T : class;

        void Reload<T>(Func<IQuery, IEnumerable<T>> loader, Expression<Func<T, object>> key)
            where T : class;
    }
}