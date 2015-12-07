namespace NuClear.CustomerIntelligence.Domain.Model.Facts
{
    public sealed class LegalPerson : IErmFactObject
    {
        public long Id { get; set; }

        public long ClientId { get; set; }
    }
}