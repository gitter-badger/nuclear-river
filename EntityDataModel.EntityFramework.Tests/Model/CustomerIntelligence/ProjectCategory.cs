namespace NuClear.AdvancedSearch.EntityDataModel.EntityFramework.Tests.Model.CustomerIntelligence
{
    public class ProjectCategory
    {
        public long ProjectId { get; set; }
        
        public long CategoryId { get; set; }

        public string Name { get; set; }

        public int Level { get; set; }

        public long? ParentId { get; set; }
    }
}