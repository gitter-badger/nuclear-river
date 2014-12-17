using System.Collections.Generic;
using System.Linq;

namespace NuClear.AdvancedSearch.EntityDataModel.OData.Building
{
    public interface IEdmModelSource
    {
        string Namespace { get; }

        IReadOnlyCollection<EdmEntityInfo> Entities { get; }
    }

    public sealed class EdmEntityInfo
    {
        private readonly List<EdmEntityPropertyInfo> _properties;
        private readonly List<EdmEntityPropertyInfo> _keys;

        public EdmEntityInfo(string name, IEnumerable<EdmEntityPropertyInfo> properties)
            : this(name, properties, null)
        {
        }

        public EdmEntityInfo(string name, IEnumerable<EdmEntityPropertyInfo> properties, IEnumerable<EdmEntityPropertyInfo> keys)
        {
            Name = name;
            _properties = properties.ToList();
            _keys = (keys ?? Enumerable.Empty<EdmEntityPropertyInfo>()).ToList();
        }

        public string Name { get; private set; }

        public IReadOnlyCollection<EdmEntityPropertyInfo> Properties { get { return _properties; } }

        public IReadOnlyCollection<EdmEntityPropertyInfo> Keys { get { return _keys; } }

        public bool HasKey
        {
            get
            {
                return Keys.Count > 0;
            }
        }
    }

    public sealed class EdmEntityPropertyInfo
    {
        public EdmEntityPropertyInfo(string name, EdmEntityPropertyType type)
            : this(name, type, true)
        {
        }

        public EdmEntityPropertyInfo(string name, EdmEntityPropertyType type, bool isNullable)
        {
            Name = name;
            Type = type;
            IsNullable = isNullable;
        }

        public string Name { get; private set; }

        public bool IsNullable { get; private set; }

        public EdmEntityPropertyType Type { get; private set; }
    }

    public sealed class EdmEntityPropertyType
    {
        public EdmEntityPropertyType(EdmEntityPropertyTypeKind typeKind)
        {
            TypeKind = typeKind;
        }

        public EdmEntityPropertyTypeKind TypeKind { get; private set; }
    }

    public enum EdmEntityPropertyTypeKind
    {
        Boolean,
        
        String,

        Byte,
        SByte,
        Int16,
        Int32,
        Int64,

        Single,
        Double,
        Decimal,
        
        Date,
        DateTimeOffset,
        TimeOfDay,

        Guid,
    }
}