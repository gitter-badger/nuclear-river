using LinqToDB.Mapping;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Bit;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data
{
    public static partial class Schema
    {
        private const string FactsBitSchema = "BIT";
        
        public static MappingSchema FactsBit
        {
            get
            {
                var schema = new MappingSchema();
                var config = schema.GetFluentMappingBuilder();

                config.Entity<FirmStatistics>().HasSchemaName(FactsBitSchema)
                      .Property(x => x.ProjectId).IsPrimaryKey()
                      .Property(x => x.FirmId).IsPrimaryKey()
                      .Property(x => x.CategoryId).IsPrimaryKey();

                config.Entity<CategoryStatististics>().HasSchemaName(FactsBitSchema)
                      .Property(x => x.ProjectId).IsPrimaryKey()
                      .Property(x => x.CategoryId).IsPrimaryKey();

                return schema;
            }
        }
    }
}