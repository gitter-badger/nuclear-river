using System.Collections;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.API.Specifications
{
    public static class Specs
    {
        public static class Find
        {
            public static FindSpecification<T> ByIds<T>(IReadOnlyCollection<long> ids) where T : IIdentifiable
            {
                return new FindSpecification<T>(x => ids.Contains(x.Id));
            }
        }

        public static class Map
        {
            public static readonly MapSpecification<IEnumerable, IEnumerable<long>> ToIds =
                new MapSpecification<IEnumerable, IEnumerable<long>>(x => x.Cast<IIdentifiable>().Select(y => y.Id));
        }
    }
}