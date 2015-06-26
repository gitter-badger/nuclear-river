using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Newtonsoft.Json;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts;

namespace NuClear.AdvancedSearch.Replication.Tests.Data
{
    public static class BitFactsContextDataExtensions
    {
        public static IBitFactsContext ToBitFactsContext(this string jsonContext)
        {
            return JsonConvert.DeserializeObject<BitFactsContext>(jsonContext) ?? new BitFactsContext();
        }

        // ReSharper disable MemberCanBePrivate.Local
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        // ReSharper disable once ClassNeverInstantiated.Local
        [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1502:ElementMustNotBeOnSingleLine", Justification = "Reviewed. Suppression is OK here.")]
        private class BitFactsContext : IBitFactsContext
        {
            public IEnumerable<FirmCategoryStatistics> FirmStatistics { get; set; }
            public IEnumerable<ProjectCategoryStatistics> CategoryStatistics { get; set; }

            IQueryable<FirmCategoryStatistics> IBitFactsContext.FirmStatistics { get { return (FirmStatistics ?? Enumerable.Empty<FirmCategoryStatistics>()).AsQueryable(); } }
            IQueryable<ProjectCategoryStatistics> IBitFactsContext.CategoryStatistics { get { return (CategoryStatistics ?? Enumerable.Empty<ProjectCategoryStatistics>()).AsQueryable(); } }
        }
        // ReSharper restore MemberCanBePrivate.Local
        // ReSharper restore UnusedAutoPropertyAccessor.Local
    }
}
