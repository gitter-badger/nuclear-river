using LinqToDB.Mapping;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data
{
    public static partial class Schema
    {
        private const string ErmSchema = "ERM";
        
        public static MappingSchema Facts
        {
            get
            {
                var schema = new MappingSchema();
                var config = schema.GetFluentMappingBuilder();

//                config.HasAttribute<Fact.CategoryFirmAddress>(new TableAttribute { Schema = ErmSchema, Name = "CategoryFirmAddresses", IsColumnAttributeRequired = false });
//                config.HasAttribute<Fact.CategoryOrganizationUnit>(new TableAttribute { Schema = ErmSchema, Name = "CategoryOrganizationUnits", IsColumnAttributeRequired = false });

                config.Entity<Account>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<CategoryFirmAddress>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<CategoryOrganizationUnit>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<Client>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<Contact>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<Firm>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<FirmAddress>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<FirmContact>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<LegalPerson>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<Order>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();

                return schema;
            }
        }
    }
}