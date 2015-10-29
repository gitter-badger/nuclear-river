using System;
using System.Linq;
using System.Reflection;
using System.Text;

using LinqToDB.Mapping;

using NuClear.DataTest.Metamodel;
using NuClear.DataTest.Runner.Smo;
using NuClear.Metamodeling.Provider;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.DataTest.Runner.Command
{
    internal sealed class ValidateDatabaseSchemataCommand : Command
    {
        private readonly ConnectionStringSettingsAspect _settings;

        public ValidateDatabaseSchemataCommand(Assembly targetAssembly, IMetadataProvider metadataProvider)
            : base(targetAssembly, metadataProvider)
        {
            _settings = TargetAssembly.GetConnectionStrings();
        }

        protected override void Execute(SchemaMetadataElement metadataElement)
        {
            IConnectionStringIdentity masterConnectionStringIdentity;
            if (!metadataElement.TryGetMasterConnectionString(out masterConnectionStringIdentity))
            {
                return;
            }

            var connectionString = _settings.GetConnectionString(metadataElement.ConnectionStringIdentity);
            var masterConnectionString = _settings.GetConnectionString(masterConnectionStringIdentity);

            var schemas = metadataElement.Entities
                                             .Select(x => metadataElement.Schema.GetAttribute<TableAttribute>(x)?.Schema)
                                             .Where(x => !string.IsNullOrEmpty(x))
                                             .Distinct();

            var databaseSchemaComparer = new DatabaseSchemaComparer(connectionString, masterConnectionString, schemas);
            var differences = databaseSchemaComparer.GetDifferences();

            var sb = new StringBuilder("Tables and vies not exist ot different in master database" + Environment.NewLine);
            var hasDifferences = false;
            foreach (var difference in differences)
            {
                hasDifferences = true;
                sb.AppendLine(difference.ToString());
            }

            if (hasDifferences)
            {
                throw new ArgumentException(sb.ToString());
            }
        }
    }
}
