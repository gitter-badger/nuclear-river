using LinqToDB.Mapping;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data
{
    public static partial class Schema
    {
        private const string ErmSchema = "ERM";
        private const string BitSchema = "BIT";

        public static MappingSchema Facts
        {
            get
            {
                var schema = new MappingSchema();
                var config = schema.GetFluentMappingBuilder();

                config.Entity<Account>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<Category>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<CategoryGroup>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<BranchOfficeOrganizationUnit>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<CategoryFirmAddress>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<CategoryOrganizationUnit>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<Client>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<Contact>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<Firm>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<FirmAddress>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<FirmContact>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<LegalPerson>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<Order>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<Project>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<Territory>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();

                config.Entity<FirmStatistics>().HasSchemaName(BitSchema)
                      .Property(x => x.ProjectId).IsPrimaryKey()
                      .Property(x => x.FirmId).IsPrimaryKey()
                      .Property(x => x.CategoryId).IsPrimaryKey();

                config.Entity<CategoryStatististics>().HasSchemaName(BitSchema)
                      .Property(x => x.ProjectId).IsPrimaryKey()
                      .Property(x => x.CategoryId).IsPrimaryKey();

                return schema;
            }
        }
    }
}