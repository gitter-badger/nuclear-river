using LinqToDB.Mapping;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data
{
    public static partial class Schema
    {
        private const string CustomerIntelligenceSchema = "CustomerIntelligence";
        
        public static MappingSchema CustomerIntelligence
        {
            get
            {
                var schema = new MappingSchema();
                var config = schema.GetFluentMappingBuilder();

                config.Entity<Client>().HasSchemaName(CustomerIntelligenceSchema);
                config.Entity<Contact>().HasSchemaName(CustomerIntelligenceSchema);
                config.Entity<Firm>().HasSchemaName(CustomerIntelligenceSchema);
                config.Entity<FirmAccount>().HasSchemaName(CustomerIntelligenceSchema)
                    .Property(x => x.AccountId).IsPrimaryKey()
                    .Property(x => x.FirmId).IsPrimaryKey();

                config.HasAttribute<FirmCategory>(new TableAttribute { Schema = CustomerIntelligenceSchema, Name = "FirmCategories", IsColumnAttributeRequired = false });
                config.HasAttribute<FirmCategoryGroup>(new TableAttribute { Schema = CustomerIntelligenceSchema, Name = "FirmCategoryGroups", IsColumnAttributeRequired = false });

                return schema;
            }
        }
    }
}