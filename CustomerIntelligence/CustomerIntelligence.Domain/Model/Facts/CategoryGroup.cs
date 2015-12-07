namespace NuClear.CustomerIntelligence.Domain.Model.Facts
{
    public sealed class CategoryGroup : IErmFactObject
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public float Rate { get; set; }
    }
}