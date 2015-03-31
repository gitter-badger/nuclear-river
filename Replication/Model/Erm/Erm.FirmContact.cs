// ReSharper disable once CheckNamespace
namespace NuClear.AdvancedSearch.Replication.Model
{
    public static partial class Erm
    {
        public sealed class FirmContact : IIdentifiable
        {
            public long Id { get; set; }

            public int ContactType { get; set; }

            public long? FirmAddressId { get; set; }
        }
    }
}