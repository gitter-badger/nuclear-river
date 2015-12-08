namespace NuClear.CustomerIntelligence.Domain.Model.Erm
{
    public sealed class SalesModelCategoryRestriction : IErmObject
    {
        public long Id { get; set; }
        public long CategoryId { get; set; }
        public long ProjectId { get; set; }
        public int SalesModel { get; set; }
    }
}