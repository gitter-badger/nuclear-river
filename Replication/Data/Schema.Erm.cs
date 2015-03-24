using System;

using LinqToDB.Mapping;

using NuClear.AdvancedSearch.Replication.Model.Erm;

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
                config.HasAttribute<Erm.LegalPerson>(new TableAttribute { Schema = BillingSchema, Name = "LegalPersons", IsColumnAttributeRequired = false });

                config.Entity<Erm.Contact>().HasSchemaName(BillingSchema).HasTableName("Contacts").Property(x => x.Role).HasColumnName("AccountRole");

                return schema;
            }
        }
    }
}