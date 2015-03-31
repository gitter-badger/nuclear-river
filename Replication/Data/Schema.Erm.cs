using System;

using LinqToDB.Mapping;

using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.Data
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

                config.HasAttribute<Erm.Account>(new TableAttribute { Schema = BillingSchema, Name = "Accounts", IsColumnAttributeRequired = false });
                config.HasAttribute<Erm.CategoryFirmAddress>(new TableAttribute { Schema = BusinessDirectorySchema, Name = "CategoryFirmAddresses", IsColumnAttributeRequired = false });
                config.HasAttribute<Erm.CategoryOrganizationUnit>(new TableAttribute { Schema = BusinessDirectorySchema, Name = "CategoryOrganizationUnits", IsColumnAttributeRequired = false });
                config.HasAttribute<Erm.Client>(new TableAttribute { Schema = BillingSchema, Name = "Clients", IsColumnAttributeRequired = false });
                config.HasAttribute<Erm.Contact>(new TableAttribute { Schema = BillingSchema, Name = "Contacts", IsColumnAttributeRequired = false });
                config.HasAttribute<Erm.Firm>(new TableAttribute { Schema = BusinessDirectorySchema, Name = "Firms", IsColumnAttributeRequired = false });
                config.HasAttribute<Erm.FirmAddress>(new TableAttribute { Schema = BusinessDirectorySchema, Name = "FirmAddresses", IsColumnAttributeRequired = false });
                config.HasAttribute<Erm.FirmContact>(new TableAttribute { Schema = BusinessDirectorySchema, Name = "FirmContacts", IsColumnAttributeRequired = false });
                config.HasAttribute<Erm.LegalPerson>(new TableAttribute { Schema = BillingSchema, Name = "LegalPersons", IsColumnAttributeRequired = false });
                config.HasAttribute<Erm.Order>(new TableAttribute { Schema = BillingSchema, Name = "Orders", IsColumnAttributeRequired = false });

                config.Entity<Erm.Account>().HasSchemaName(BillingSchema).HasTableName("Accounts").Property(x => x.Id).IsPrimaryKey();
                config.Entity<Erm.CategoryFirmAddress>().HasSchemaName(BillingSchema).HasTableName("CategoryFirmAddresses").Property(x => x.Id).IsPrimaryKey();
                config.Entity<Erm.CategoryOrganizationUnit>().HasSchemaName(BillingSchema).HasTableName("CategoryOrganizationUnits").Property(x => x.Id).IsPrimaryKey();
                config.Entity<Erm.Client>().HasSchemaName(BillingSchema).HasTableName("Clients").Property(x => x.Id).IsPrimaryKey();
                config.Entity<Erm.Contact>()
                    .HasSchemaName(BillingSchema)
                    .HasTableName("Contacts")
                    .Property(x => x.Id).IsPrimaryKey()
                    .Property(x => x.Role).HasColumnName("AccountRole")
                    ;
                config.Entity<Erm.Firm>().HasSchemaName(BusinessDirectorySchema).HasTableName("Firms").Property(x => x.Id).IsPrimaryKey();
                config.Entity<Erm.FirmAddress>().HasSchemaName(BusinessDirectorySchema).HasTableName("FirmAddresses").Property(x => x.Id).IsPrimaryKey();
                config.Entity<Erm.FirmContact>().HasSchemaName(BusinessDirectorySchema).HasTableName("FirmContacts").Property(x => x.Id).IsPrimaryKey();
                config.Entity<Erm.LegalPerson>().HasSchemaName(BillingSchema).HasTableName("LegalPersons").Property(x => x.Id).IsPrimaryKey();
                config.Entity<Erm.Order>().HasSchemaName(BillingSchema).HasTableName("Orders").Property(x => x.Id).IsPrimaryKey();

                return schema;
            }
        }
    }
}