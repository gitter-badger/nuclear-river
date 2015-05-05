using LinqToDB.Mapping;

using NuClear.OperationsProcessing.Transports.SQLStore.Final;

namespace NuClear.Replication.OperationsProcessing.Transports.SQLStore
{
    public static class Schema
    {
        private const string TransportSchema = "Transport";

        public static MappingSchema Transport
        {
            get
            {
                var schema = new MappingSchema();
                var config = schema.GetFluentMappingBuilder();

                config.HasAttribute<PerformedOperationFinalProcessing>(new TableAttribute { Schema = TransportSchema, Name = "PerformedOperationFinalProcessings", IsColumnAttributeRequired = false });

                config.Entity<PerformedOperationFinalProcessing>()
                      .HasSchemaName(TransportSchema)
                      .HasTableName("PerformedOperationFinalProcessings")
                      .Property(x => x.Id)
                      .IsPrimaryKey()
                      .IsIdentity();

                return schema;
            }
        }
    }
}