namespace NuClear.AdvancedSearch.EntityDataModel.EntityFramework.Tests.Model.CustomerIntelligence
{
    public class FirmCategory
    {
        public long Id { get; set; }
        public Category Category { get; set; }
        public CategoryGroup CategoryGroup { get; set; }
    }
}