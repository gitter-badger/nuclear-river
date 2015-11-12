using System.Reflection;

using NuClear.DataTest.Metamodel;
using NuClear.Metamodeling.Provider;

namespace NuClear.DataTest.Runner.Command
{
    public sealed class DropDatabasesCommand : Command
    {
        private readonly SmoConnectionFactory _smoConnectionFactory;

        public DropDatabasesCommand(Assembly targetAssembly, IMetadataProvider metadataProvider, SmoConnectionFactory smoConnectionFactory)
            : base(targetAssembly, metadataProvider)
        {
            _smoConnectionFactory = smoConnectionFactory;
        }

        protected override void Execute(SchemaMetadataElement metadataElement)
        {
            var existingDb = _smoConnectionFactory.CreateDatabaseConnection(metadataElement.ConnectionStringIdentity);
            existingDb?.Parent.KillDatabase(existingDb.Name);
        }
    }
}