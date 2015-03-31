// ReSharper disable once CheckNamespace
namespace NuClear.AdvancedSearch.Replication.Model
{
    public static partial class Erm
    {
        public sealed class Account : IIdentifiable
        {
            public Account()
            {
                IsActive = true;
            }

            public long Id { get; set; }

            public decimal Balance { get; set; }

            public long LegalPersonId { get; set; }

            public bool IsActive { get; set; }

            public bool IsDeleted { get; set; }
        }
    }
}