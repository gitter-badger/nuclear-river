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

//                config.HasAttribute<Fact.CategoryFirmAddress>(new TableAttribute { Schema = ErmSchema, Name = "CategoryFirmAddresses", IsColumnAttributeRequired = false });
//                config.HasAttribute<Fact.CategoryOrganizationUnit>(new TableAttribute { Schema = ErmSchema, Name = "CategoryOrganizationUnits", IsColumnAttributeRequired = false });

                config.Entity<Fact.Account>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<Fact.CategoryFirmAddress>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<Fact.CategoryOrganizationUnit>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<Fact.Client>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<Fact.Contact>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<Fact.Firm>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<Fact.FirmAddress>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<Fact.FirmContact>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<Fact.LegalPerson>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<Fact.Order>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();

                return schema;
            }
        }
    }
}