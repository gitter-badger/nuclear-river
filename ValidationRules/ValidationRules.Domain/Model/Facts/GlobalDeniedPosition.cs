namespace NuClear.ValidationRules.Domain.Model.Facts
{
    public sealed class GlobalDeniedPosition : IConfigFactObject
    {
        public long MasterPositionId { get; set; }
        public long DeniedPositionId { get; set; }
        public int ObjectBindingType { get; set; }
    }
}