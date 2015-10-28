using LinqToDB.Mapping;

using NuClear.Metamodeling.Elements.Aspects.Features;

namespace NuClear.DataTest.Metamodel
{
    public class Linq2DbSchemaFeature : IUniqueMetadataFeature
    {
        public Linq2DbSchemaFeature(MappingSchema schema)
        {
            Schema = schema;
        }

        public MappingSchema Schema { get; }
    }
}