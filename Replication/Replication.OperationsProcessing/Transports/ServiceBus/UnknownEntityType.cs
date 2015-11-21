using System;

using NuClear.Model.Common;
using NuClear.Model.Common.Entities;

namespace NuClear.Replication.OperationsProcessing.Transports.ServiceBus
{
    public sealed class UnknownEntityType : IEntityType, IEquatable<UnknownEntityType>
    {
        private readonly int _id;
        public int Id { get { return _id; } }

        public UnknownEntityType(int id)
        {
            _id = id;
        }

        public bool Equals(UnknownEntityType other)
        {
            if (other == null)
            {
                return false;
            }

            return Id == other.Id;
        }

        public bool Equals(IIdentity other)
        {
            return Equals(other as UnknownEntityType);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as UnknownEntityType);
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public override string ToString()
        {
            return string.Format("Id={0}. Description: {1}", Id, Description);
        }

        public string Description { get { return "Unknown"; } }
    }
}
