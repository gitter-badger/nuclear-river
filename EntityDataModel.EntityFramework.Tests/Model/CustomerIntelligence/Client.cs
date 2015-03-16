using System.Collections.Generic;

namespace NuClear.AdvancedSearch.EntityDataModel.EntityFramework.Tests.Model.CustomerIntelligence
{
    public class Client
    {
        public long Id { get; set; }
        public string Name { get; set; }
        
        public long CategoryGroupId { get; set; }

        public ICollection<Contact> Contacts { get; set; }
    }
}