using System.Collections.Generic;

namespace NuClear.AdvancedSearch.EntityDataModel.Metadata.Features
{
    public sealed class EntityPropertyEnumTypeFeature : EntityPropertyTypeFeature
    {
        private readonly IReadOnlyDictionary<string, long> _members;

        public EntityPropertyEnumTypeFeature(string name, EntityPropertyType underlyingType, IReadOnlyDictionary<string,long> members)
            : base(EntityPropertyType.Enum)
        {
            Name = name;
            UnderlyingType = underlyingType;
            _members = members;
        }

        public string Name { get; private set; }

        public EntityPropertyType UnderlyingType { get; private set; }

        public IReadOnlyDictionary<string, long> Members { get { return _members; } }
    }
}