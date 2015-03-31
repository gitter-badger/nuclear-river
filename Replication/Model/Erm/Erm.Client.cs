using System;

// ReSharper disable once CheckNamespace
namespace NuClear.AdvancedSearch.Replication.Model
{
    public static partial class Erm
    {
        public sealed class Client : IIdentifiable
        {
            public Client()
            {
                IsActive = true;
            }

            public long Id { get; set; }

            public string Name { get; set; }

            public DateTimeOffset? LastDisqualifyTime { get; set; }

            public string MainPhoneNumber { get; set; }

            public string AdditionalPhoneNumber1 { get; set; }

            public string AdditionalPhoneNumber2 { get; set; }

            public string Website { get; set; }

            public bool IsActive { get; set; }

            public bool IsDeleted { get; set; }
        }
    }
}