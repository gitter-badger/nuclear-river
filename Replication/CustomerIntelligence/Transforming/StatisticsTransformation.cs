using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    public class StatisticsTransformation
    {
        private readonly ICustomerIntelligenceContext _source;
        private readonly ICustomerIntelligenceContext _target;

        public StatisticsTransformation(ICustomerIntelligenceContext source, ICustomerIntelligenceContext target)
        {
            _source = source;
            _target = target;
        }

        public IEnumerable<StatisticsOperation> DetectStatisticsOperations(IEnumerable<FactOperation> enumerable)
        {
            return new[]
                   {
                       new StatisticsOperation { ProjectId = 1 },
                       new StatisticsOperation { ProjectId = 1, CategoryId = 1 },
                   };
        }

        public void Recalculate(IEnumerable<StatisticsOperation> operations)
        {
            foreach(var project in operations.GroupBy(x => x.ProjectId, x => x.CategoryId))
            {
                //var projectId = project.Key;
                //Expression<Func<ProjectCategory, bool>> projectSpecification = p => p.ProjectId == projectId;

                //var categoryIds = project.Distinct().ToList();
                //Expression<Func<FirmCategory, bool>> categorySpecification;
                //if (categoryIds.Any(x => x == null))
                //{
                //    categorySpecification = p => true;
                //}
                //else
                //{
                //    categorySpecification = p => categoryIds.Contains(p.CategoryId);
                //}

                //var z = _source.ProjectCategories.Where(projectSpecification).Where(categorySpecification);
            }
        }
    }
}
