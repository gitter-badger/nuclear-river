using System.Collections.Generic;

namespace NuClear.Querying.EntityFramework.Tests.Model.CustomerIntelligence
{
    public class Client
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public CategoryGroup CategoryGroup { get; set; }
        public ICollection<ClientContact> Contacts { get; set; }
    }
}