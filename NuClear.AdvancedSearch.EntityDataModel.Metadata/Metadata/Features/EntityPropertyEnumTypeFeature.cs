using System.Collections.Generic;

namespace NuClear.AdvancedSearch.EntityDataModel.Metadata.Features
{
    public sealed class EntityPropertyEnumTypeFeature : EntityPropertyTypeFeature
    {
        private readonly IReadOnlyDictionary<string, object> _members;

        public EntityPropertyEnumTypeFeature(EntityPropertyType underlyingType, IReadOnlyDictionary<string,object> members)
            : base(EntityPropertyType.Enum)
        {
            UnderlyingType = underlyingType;
            _members = members;
        }

        public EntityPropertyType UnderlyingType { get; private set; }

        public IReadOnlyDictionary<string, object> Members { get { return _members; } }
    }
}