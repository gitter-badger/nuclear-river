using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.Specifications
{
    public static partial class Specs
    {
        public static class CI
        {
            public static class Map
            {
                public static MapSpecification<IQuery, IQueryable<ClientContact>> ClientContacts(IEnumerable<long> aggregateIds)
                {
                    return new MapSpecification<IQuery, IQueryable<ClientContact>>(
                        q => from clientContact in q.For<ClientContact>()
                             where aggregateIds.Contains(clientContact.ClientId)
                             select clientContact);
                }

                public static MapSpecification<IQuery, IQueryable<FirmBalance>> FirmBalances(IEnumerable<long> aggregateIds)
                {
                    return new MapSpecification<IQuery, IQueryable<FirmBalance>>(
                        q => from firmBalance in q.For<FirmBalance>()
                             where aggregateIds.Contains(firmBalance.FirmId)
                             select firmBalance);
                }

                public static MapSpecification<IQuery, IQueryable<FirmCategory>> FirmCategories(IEnumerable<long> aggregateIds)
                {
                    return new MapSpecification<IQuery, IQueryable<FirmCategory>>(
                        q => from firmCategory in q.For<FirmCategory>()
                             where aggregateIds.Contains(firmCategory.FirmId)
                             select firmCategory);
                }

                public static MapSpecification<IQuery, IQueryable<ProjectCategory>> ProjectCategories(IEnumerable<long> aggregateIds)
                {
                    return new MapSpecification<IQuery, IQueryable<ProjectCategory>>(
                        q => from projectCategory in q.For<ProjectCategory>()
                             where aggregateIds.Contains(projectCategory.ProjectId)
                             select projectCategory);
                }
            }
        }
    }
}