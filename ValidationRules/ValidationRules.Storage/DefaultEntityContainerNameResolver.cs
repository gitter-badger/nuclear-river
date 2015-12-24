using System;

using NuClear.Storage.Core;

namespace NuClear.ValidationRules.Storage
{
    public class DefaultEntityContainerNameResolver : IEntityContainerNameResolver
    {
        private const string Erm = "Erm";
        private const string Facts = "Facts";

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

            throw new ArgumentException($"Unsupported type {objType.Name}: can not determine scope", nameof(objType));
        }
    }
}