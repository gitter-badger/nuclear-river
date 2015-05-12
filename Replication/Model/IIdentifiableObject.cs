namespace NuClear.AdvancedSearch.Replication.Model
{
    public interface IIdentifiableObject : IObject
    {
        long Id { get; }
    }
}