// ReSharper disable once CheckNamespace
namespace NuClear.AdvancedSearch.Replication.Model
{
    public static partial class Erm
    {
        public sealed class LegalPerson : IIdentifiable
        {
            public LegalPerson()
            {
                IsActive = true;
            }

            public long Id { get; set; }

            public long? ClientId { get; set; }

            public bool IsActive { get; set; }

            public bool IsDeleted { get; set; }
        }
    }
}