using System.Collections.Generic;

namespace NuClear.AdvancedSearch.EntityDataModel.EntityFramework.Tests.Model.CustomerIntelligence
{
    public class Client
    {
        public long Id { get; set; }
        public long CategoryGroup { get; set; }

        public ICollection<Account> Accounts { get; set; }
        public ICollection<Contact> Contacts { get; set; }
    }
}