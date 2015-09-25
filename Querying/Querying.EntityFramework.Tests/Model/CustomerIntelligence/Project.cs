using System.Collections.Generic;

namespace NuClear.Querying.EntityFramework.Tests.Model.CustomerIntelligence
{
    public class Project
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public ICollection<ProjectCategory> Categories { get; set; }
        public ICollection<Territory> Territories { get; set; }
        public ICollection<Firm> Firms { get; set; }
    }
}