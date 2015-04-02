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

                config.HasAttribute<FirmCategory>(new TableAttribute { Schema = CustomerIntelligenceSchema, Name = "FirmCategories", IsColumnAttributeRequired = false });
                config.HasAttribute<FirmCategoryGroup>(new TableAttribute { Schema = CustomerIntelligenceSchema, Name = "FirmCategoryGroups", IsColumnAttributeRequired = false });

                config.Entity<Client>().HasSchemaName(CustomerIntelligenceSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<Contact>().HasSchemaName(CustomerIntelligenceSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<Firm>().HasSchemaName(CustomerIntelligenceSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<FirmBalance>().HasSchemaName(CustomerIntelligenceSchema)
                    .Property(x => x.AccountId).IsPrimaryKey()
                    .Property(x => x.FirmId).IsPrimaryKey();
                config.Entity<FirmCategory>().HasSchemaName(CustomerIntelligenceSchema)
                    .Property(x => x.CategoryId).IsPrimaryKey()
                    .Property(x => x.FirmId).IsPrimaryKey();
                config.Entity<FirmCategoryGroup>().HasSchemaName(CustomerIntelligenceSchema)
                    .Property(x => x.CategoryGroupId).IsPrimaryKey()
                    .Property(x => x.FirmId).IsPrimaryKey();

                return schema;
            }
        }
    }
}