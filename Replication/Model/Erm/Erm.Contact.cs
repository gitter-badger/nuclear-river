// ReSharper disable once CheckNamespace
namespace NuClear.AdvancedSearch.Replication.Model
{
    public static partial class Erm
    {
        public sealed class Contact : IIdentifiable
        {
            public Contact()
            {
                IsActive = true;
            }

            public long Id { get; set; }

            public int Role { get; set; }

            public bool IsFired { get; set; }

            public string MainPhoneNumber { get; set; }

            public string AdditionalPhoneNumber { get; set; }

            public string MobilePhoneNumber { get; set; }

            public string HomePhoneNumber { get; set; }

            public string Website { get; set; }

            public long ClientId { get; set; }

            public bool IsActive { get; set; }

            public bool IsDeleted { get; set; }
        }
    }
}