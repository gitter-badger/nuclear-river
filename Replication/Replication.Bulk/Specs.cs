using NuClear.Storage.API.Specifications;

namespace NuClear.AdvancedSearch.Replication.Bulk
{
    public static class Specs
    {
        public static class Find
        {
            public static FindSpecification<T> All<T>()
            {
                return new FindSpecification<T>(x => true);
            }
        }
    }
}