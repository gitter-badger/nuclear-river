namespace NuClear.AdvancedSearch.EntityDataModel.EntityFramework.Tests.Model.BusinessDirectory
{
    public class Territory
    {
        public long Id { get; set; }
        public string Name { get; set; }
        
        public OrganizationUnit OrganizationUnit { get; set; }
    }
}