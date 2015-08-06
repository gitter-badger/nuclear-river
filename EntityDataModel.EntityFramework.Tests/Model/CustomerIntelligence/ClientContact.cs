namespace NuClear.AdvancedSearch.EntityDataModel.EntityFramework.Tests.Model.CustomerIntelligence
{
    public class ClientContact
    {
        public long ContactId { get; set; }
        public long ClientId { get; set; }
        public ContactRole Role { get; set; }
        public bool IsFired { get; set; }
    }
}