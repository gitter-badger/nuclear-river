using System;
using System.Configuration;

using LinqToDB.Data;
using LinqToDB.Mapping;

namespace NuClear.AdvancedSearch.Replication.Bulk
{
	public sealed class Storage
	{
		public Storage(string connectionStringName, MappingSchema mappingSchema)
		{
			ConnectionStringName = connectionStringName;
			MappingSchema = mappingSchema;
		}

		public string ConnectionStringName { get; }
		public MappingSchema MappingSchema { get; }

		public DataConnection CreateConnection()
		{
			var connectionString = ConfigurationManager.ConnectionStrings[ConnectionStringName];
			var connection = new DataConnection(connectionString.ProviderName, connectionString.ConnectionString);
			connection.AddMappingSchema(MappingSchema);
			connection.CommandTimeout = (int)TimeSpan.FromMinutes(30).TotalMilliseconds;
			return connection;
		}
	}
}