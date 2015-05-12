namespace NuClear.AdvancedSearch.EntityDataModel.EntityFramework.Tests.Model.CustomerIntelligence
{
    public class ProjectCategory
    {
        public long ProjectId { get; set; }
        
        public long CategoryId { get; set; }

        public double AdvertisersShare { get; set; }

        public long FirmCount { get; set; }

        public Category Category { get; set; }
    }
}