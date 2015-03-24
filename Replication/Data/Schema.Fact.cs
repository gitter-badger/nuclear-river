using LinqToDB.Mapping;

using NuClear.AdvancedSearch.Replication.Model.CustomerIntelligence;

namespace NuClear.AdvancedSearch.Replication.Data
{
    public static partial class Schema
    {
        private const string ErmSchema = "ERM";
        
        public static MappingSchema Fact
        {
            get
            {
                var schema = new MappingSchema();
                var config = schema.GetFluentMappingBuilder();

                config.HasAttribute<Fact.FirmCategory>(new TableAttribute { Schema = ErmSchema, Name = "FirmCategories", IsColumnAttributeRequired = false });
                config.HasAttribute<Fact.FirmCategoryGroup>(new TableAttribute { Schema = ErmSchema, Name = "FirmCategoryGroups", IsColumnAttributeRequired = false });

                config.Entity<Fact.Client>().HasSchemaName(ErmSchema);
                config.Entity<Fact.Contact>().HasSchemaName(ErmSchema);
                config.Entity<Fact.Firm>().HasSchemaName(ErmSchema);
                config.Entity<Fact.FirmAccount>().HasSchemaName(ErmSchema);
                config.Entity<Fact.FirmCategory>().HasSchemaName(ErmSchema);
                config.Entity<Fact.FirmCategoryGroup>().HasSchemaName(ErmSchema);

                return schema;
            }
        }
    }
}