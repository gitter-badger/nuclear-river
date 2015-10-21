using System.Reflection;

using NuClear.DataTest.Runner.Command;
using NuClear.Metamodeling.Processors;
using NuClear.Metamodeling.Provider;

namespace NuClear.DataTest.Runner
{
    static class Program
    {
        public static void Main(string[] args)
        {
            var assemblyRef = AssemblyName.GetAssemblyName(args[0]);
            var assembly = Assembly.Load(assemblyRef);
            var metadataProvider = new MetadataProvider(assembly.GetMetadataSources(), new IMetadataProcessor[0]);
            var command1 = new CreateDatabasesCommand(assembly, metadataProvider);
            var command2 = new CreateDatabaseSchemataCommand(assembly, metadataProvider);
            command1.Execute();
            command2.Execute();
        }
    }
}
