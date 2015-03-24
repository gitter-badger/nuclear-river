namespace NuClear.AdvancedSearch.Replication.Transforming
{
    public sealed class ChangeDescription
    {
        public ChangeDescription(int entityCode, long entityId, ChangeKind changeKind)
        {
            EntityCode = entityCode;
            EntityId = entityId;
            ChangeKind = changeKind;
        }

        public int EntityCode { get; set; }

        public long EntityId { get; set; }

        public ChangeKind ChangeKind { get; set; }
    }
}