using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.SqlServer;
using LinqToDB.SqlQuery;

using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.DataTest.Metamodel
{
    public sealed class DataConnectionFactory
    {
        private readonly ConnectionStringSettingsAspect _connectionStringSettings;

        public DataConnectionFactory(ConnectionStringSettingsAspect connectionStringSettings)
        {
            _connectionStringSettings = connectionStringSettings;
        }

        public DataConnection CreateConnection(SchemaMetadataElement metadata)
        {
            var connectionString = _connectionStringSettings.GetConnectionString(metadata.ConnectionStringIdentity);

            var baseSchema = metadata.Schema;
            baseSchema.SetDataType(typeof(decimal), new SqlDataType(DataType.Decimal, 19, 4));
            baseSchema.SetDataType(typeof(string), new SqlDataType(DataType.NVarChar, int.MaxValue));

            return SqlServerTools.CreateDataConnection(connectionString).AddMappingSchema(baseSchema);
        }
    }
}
