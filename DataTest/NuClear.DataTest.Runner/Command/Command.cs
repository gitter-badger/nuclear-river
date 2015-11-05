using System.Reflection;

using NuClear.DataTest.Metamodel;
using NuClear.Metamodeling.Provider;

namespace NuClear.DataTest.Runner.Command
{
    public abstract class Command : ICommand
    {
        protected Command(Assembly targetAssembly, IMetadataProvider metadataProvider)
        {
            TargetAssembly = targetAssembly;
            MetadataSet = metadataProvider.GetMetadataSet<SchemaMetadataIdentity>();
        }

        protected MetadataSet MetadataSet { get; }

        protected Assembly TargetAssembly { get; }

        public void Execute()
        {
            foreach (var metadataElement in MetadataSet.Metadata)
            {
                Execute((SchemaMetadataElement)metadataElement.Value);
            }
        }

        protected abstract void Execute(SchemaMetadataElement metadataElement);
    }
}
