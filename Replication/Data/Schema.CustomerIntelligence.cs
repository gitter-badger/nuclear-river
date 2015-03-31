using LinqToDB.Mapping;

using NuClear.AdvancedSearch.Replication.Model.CustomerIntelligence;

namespace NuClear.AdvancedSearch.Replication.Data
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

                config.Entity<CustomerIntelligence.Client>().HasSchemaName(CustomerIntelligenceSchema);
                config.Entity<CustomerIntelligence.Contact>().HasSchemaName(CustomerIntelligenceSchema);
                config.Entity<CustomerIntelligence.Firm>().HasSchemaName(CustomerIntelligenceSchema);
                config.Entity<CustomerIntelligence.FirmAccount>().HasSchemaName(CustomerIntelligenceSchema)
                    .Property(x => x.AccountId).IsPrimaryKey()
                    .Property(x => x.FirmId).IsPrimaryKey();

                config.HasAttribute<CustomerIntelligence.FirmCategory>(new TableAttribute { Schema = CustomerIntelligenceSchema, Name = "FirmCategories", IsColumnAttributeRequired = false });
                config.HasAttribute<CustomerIntelligence.FirmCategoryGroup>(new TableAttribute { Schema = CustomerIntelligenceSchema, Name = "FirmCategoryGroups", IsColumnAttributeRequired = false });

                return schema;
            }
        }
    }
}