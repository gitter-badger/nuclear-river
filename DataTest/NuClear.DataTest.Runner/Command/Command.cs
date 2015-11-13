using NuClear.DataTest.Metamodel;
using NuClear.Metamodeling.Provider;

namespace NuClear.DataTest.Runner.Command
{
    public abstract class Command : ICommand
    {
        protected Command(IMetadataProvider metadataProvider)
        {
            MetadataSet = metadataProvider.GetMetadataSet<SchemaMetadataIdentity>();
        }

        protected MetadataSet MetadataSet { get; }

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
