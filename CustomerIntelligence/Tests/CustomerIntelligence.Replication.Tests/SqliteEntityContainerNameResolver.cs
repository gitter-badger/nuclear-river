using System;

using NuClear.Storage.Core;

namespace NuClear.CustomerIntelligence.Replication.Tests
{
    public class SqliteEntityContainerNameResolver : IEntityContainerNameResolver
    {
        private const string Erm = "Erm";
        private const string Facts = "Facts";
        private const string CustomerIntelligence = "CustomerIntelligence";

        public string Resolve(Type objType)
        {
            if (objType.Namespace.Contains(Erm))
            {
                return Erm;
            }

            if (objType.Namespace.Contains(Facts))
            {
                return Facts;
            }

            return CustomerIntelligence;
        }
    }
}