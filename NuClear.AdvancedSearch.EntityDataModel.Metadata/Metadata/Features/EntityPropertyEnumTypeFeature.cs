using System;
using System.Collections.Generic;

namespace NuClear.AdvancedSearch.EntityDataModel.Metadata.Features
{
    public sealed class EntityPropertyEnumTypeFeature : EntityPropertyTypeFeature
    {
        public EntityPropertyEnumTypeFeature(string name, EntityPropertyType underlyingType, IReadOnlyDictionary<string, long> members)
            : base(EntityPropertyType.Enum)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("The enum name was not specified.", "name");
            }

            Name = name;
            UnderlyingType = underlyingType;
            Members = members;
        }

        public string Name { get; private set; }

        public EntityPropertyType UnderlyingType { get; private set; }

        public IReadOnlyDictionary<string, long> Members { get; private set; }
    }
}