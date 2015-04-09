using System;

using LinqToDB.Mapping;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Erm;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data
{
    public static partial class Schema
    {
        private const string BillingSchema = "Billing";
        private const string BusinessDirectorySchema = "BusinessDirectory";

        public static MappingSchema Erm
        {
            get
            {
                var schema = new MappingSchema();

                // NOTE: ERM хранит даты в UTC с использованием типа datetime, уберем неодназначность выставив UTC явно
                schema.SetConvertExpression<DateTime, DateTime>(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));
                schema.SetConvertExpression<DateTime, DateTimeOffset>(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));

                var config = schema.GetFluentMappingBuilder();

                config.HasAttribute<Account>(new TableAttribute { Schema = BillingSchema, Name = "Accounts", IsColumnAttributeRequired = false });
                config.HasAttribute<BranchOfficeOrganizationUnit>(new TableAttribute { Schema = BillingSchema, Name = "BranchOfficeOrganizationUnits", IsColumnAttributeRequired = false });
                config.HasAttribute<Category>(new TableAttribute { Schema = BusinessDirectorySchema, Name = "Categories", IsColumnAttributeRequired = false });
                config.HasAttribute<CategoryFirmAddress>(new TableAttribute { Schema = BusinessDirectorySchema, Name = "CategoryFirmAddresses", IsColumnAttributeRequired = false });
                config.HasAttribute<CategoryOrganizationUnit>(new TableAttribute { Schema = BusinessDirectorySchema, Name = "CategoryOrganizationUnits", IsColumnAttributeRequired = false });
                config.HasAttribute<Client>(new TableAttribute { Schema = BillingSchema, Name = "Clients", IsColumnAttributeRequired = false });
                config.HasAttribute<Contact>(new TableAttribute { Schema = BillingSchema, Name = "Contacts", IsColumnAttributeRequired = false });
                config.HasAttribute<Firm>(new TableAttribute { Schema = BusinessDirectorySchema, Name = "Firms", IsColumnAttributeRequired = false });
                config.HasAttribute<FirmAddress>(new TableAttribute { Schema = BusinessDirectorySchema, Name = "FirmAddresses", IsColumnAttributeRequired = false });
                config.HasAttribute<FirmContact>(new TableAttribute { Schema = BusinessDirectorySchema, Name = "FirmContacts", IsColumnAttributeRequired = false });
                config.HasAttribute<LegalPerson>(new TableAttribute { Schema = BillingSchema, Name = "LegalPersons", IsColumnAttributeRequired = false });
                config.HasAttribute<Order>(new TableAttribute { Schema = BillingSchema, Name = "Orders", IsColumnAttributeRequired = false });

                config.Entity<Account>().HasSchemaName(BillingSchema).HasTableName("Accounts").Property(x => x.Id).IsPrimaryKey();
                config.Entity<BranchOfficeOrganizationUnit>().HasSchemaName(BillingSchema).HasTableName("BranchOfficeOrganizationUnits").Property(x => x.Id).IsPrimaryKey();
                config.Entity<Category>().HasSchemaName(BusinessDirectorySchema).HasTableName("Categories").Property(x => x.Id).IsPrimaryKey();
                config.Entity<CategoryFirmAddress>().HasSchemaName(BusinessDirectorySchema).HasTableName("CategoryFirmAddresses").Property(x => x.Id).IsPrimaryKey();
                config.Entity<CategoryOrganizationUnit>().HasSchemaName(BusinessDirectorySchema).HasTableName("CategoryOrganizationUnits").Property(x => x.Id).IsPrimaryKey();
                config.Entity<Client>().HasSchemaName(BillingSchema).HasTableName("Clients").Property(x => x.Id).IsPrimaryKey();
                config.Entity<Contact>()
                    .HasSchemaName(BillingSchema)
                    .HasTableName("Contacts")
                    .Property(x => x.Id).IsPrimaryKey()
                    .Property(x => x.Role).HasColumnName("AccountRole")
                    ;
                config.Entity<Firm>()
                    .HasSchemaName(BusinessDirectorySchema)
                    .HasTableName("Firms")
                    .Property(x => x.Id).IsPrimaryKey()
                    .Property(x => x.OwnerId).HasColumnName("OwnerCode");
                config.Entity<FirmAddress>().HasSchemaName(BusinessDirectorySchema).HasTableName("FirmAddresses").Property(x => x.Id).IsPrimaryKey();
                config.Entity<FirmContact>().HasSchemaName(BusinessDirectorySchema).HasTableName("FirmContacts").Property(x => x.Id).IsPrimaryKey();
                config.Entity<LegalPerson>().HasSchemaName(BillingSchema).HasTableName("LegalPersons").Property(x => x.Id).IsPrimaryKey();
                config.Entity<Order>().HasSchemaName(BillingSchema).HasTableName("Orders").Property(x => x.Id).IsPrimaryKey();

                return schema;
            }
        }
    }
}