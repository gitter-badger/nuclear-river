namespace NuClear.AdvancedSearch.EntityDataModel.EntityFramework.Tests.Model.CustomerIntelligence
{
    public class Contact
    {
        public long Id { get; set; }
        public ContactRole Role { get; set; }
        public bool IsFired { get; set; }
    }
}