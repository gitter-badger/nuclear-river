using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.Model;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.Specifications
{
    public static partial class Specs
    {
        public static class Find
        {
            public static FindSpecification<T> ByIds<T>(IEnumerable<long> ids) where T : IIdentifiable
            {
                return new FindSpecification<T>(x => ids.Contains(x.Id));
            }

            public static FindSpecification<long> ByIds(IEnumerable<long> ids)
            {
                return new FindSpecification<long>(x => ids.Contains(x));
            }
        }
    }
}