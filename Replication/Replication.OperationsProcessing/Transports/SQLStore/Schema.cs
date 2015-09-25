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

                config.Entity<PerformedOperationFinalProcessing>()
                      .HasSchemaName(TransportSchema)
                      .Property(x => x.Id)
                      .IsPrimaryKey()
                      .IsIdentity();

                return schema;
            }
        }
    }
}