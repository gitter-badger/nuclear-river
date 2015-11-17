using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using LinqToDB;
using LinqToDB.Data;

using NuClear.DataTest.Metamodel;
using NuClear.DataTest.Metamodel.Dsl;
using NuClear.Metamodeling.Provider;
using NuClear.Storage.API.ConnectionStrings;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace CustomerIntelligence.Replication.DataTest.Model
{
    public sealed class BulkReplicationAdapter<T> : ITestAction
        where T : IKey, new()
    {
        private static readonly IReadOnlyDictionary<string, string> ConnectionStringMapping
            = new Dictionary<string, string>
              {
                  { "Erm", "Erm" },
                  { "Facts", "Facts" },
                  { "Bit", "Facts" },
                  { "CustomerIntelligence", "CustomerIntelligence" },
                  { "Statistics", "CustomerIntelligence" }
              };

        private readonly ActMetadataElement _metadata;
        private readonly ConnectionStringSettingsAspect _connectionStringSettingsAspect;
        private readonly CommandlineParameters _commandlineParameters;
        private readonly IDictionary<string, SchemaMetadataElement> _schemaMetadata;


        public BulkReplicationAdapter(ActMetadataElement metadata, IMetadataProvider metadataProvider, ConnectionStringSettingsAspect connectionStringSettingsAspect, CommandlineParameters commandlineParameters)
        {
            _metadata = metadata;
            _connectionStringSettingsAspect = connectionStringSettingsAspect;
            _commandlineParameters = commandlineParameters;
            _schemaMetadata = metadataProvider.GetMetadataSet<SchemaMetadataIdentity>().Metadata.Values.Cast<SchemaMetadataElement>().ToDictionary(x => x.Context, x => x);
        }

        public void Act()
        {
            string exePath;
            if (!_commandlineParameters.TryGet("BulkTool", out exePath))
            {
                throw new Exception("BulkTool parameter not found");
            }

            var fileInfo = new FileInfo(exePath);
            if (!fileInfo.Exists)
            {
                throw new Exception($"BulkTool not found at path {fileInfo.FullName}");
            }

            var oldConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            try
            {
                var contexts = new [] { _metadata.Source, _metadata.Target };
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var connectionStringsSection = (ConnectionStringsSection)config.GetSection("connectionStrings");
                connectionStringsSection.ConnectionStrings.Clear();

                foreach (var context in contexts)
                {
                    var targetConnectionStringName = ConnectionStringMapping[context];
                    var connectionString = _connectionStringSettingsAspect.GetConnectionString(_schemaMetadata[context].ConnectionStringIdentity);
                    connectionStringsSection.ConnectionStrings.Add(new ConnectionStringSettings(targetConnectionStringName, connectionString, "SqlServer"));
                }

                config.Save();

                var childDomain = AppDomain.CreateDomain("bulk-tool", null, fileInfo.DirectoryName, "", false);
                childDomain.ExecuteAssembly(fileInfo.FullName, new[] { new T().Key });
            }
            finally 
            {
                oldConfig.Save();
            }
        }
    }

    public interface IKey
    {
        string Key { get; }
    }

    public sealed class Facts : IKey
    {
        public string Key => "-fact";
    }
    public sealed class CustomerIntelligence : IKey
    {
        public string Key => "-ci";
    }
    public sealed class Statistics : IKey
    {
        public string Key => "-statistics";
    }
}