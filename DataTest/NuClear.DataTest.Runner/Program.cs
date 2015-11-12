using System;
using System.Configuration;
using System.Linq;
using System.Reflection;

using Microsoft.Practices.Unity;

using NuClear.DataTest.Metamodel;
using NuClear.DataTest.Runner.Command;
using NuClear.Metamodeling.Processors;
using NuClear.Metamodeling.Provider;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.DataTest.Runner
{
    static class Program
    {
        public static void Main(string[] args)
        {
            var container = new UnityContainer();

            var assembly = Assembly.Load(AssemblyName.GetAssemblyName(args[0]));
            var metadataProvider = new MetadataProvider(assembly.GetExportedTypes()
                                                                .Where(t => typeof(IMetadataSource).IsAssignableFrom(t))
                                                                .Select(x => (IMetadataSource)container.Resolve(x))
                                                                .ToArray(),
                                                        new IMetadataProcessor[0]);

            container.RegisterInstance<Assembly>(assembly);
            container.RegisterInstance<IMetadataProvider>(metadataProvider);
            container.RegisterInstance<Configuration>(ConfigurationManager.OpenExeConfiguration(assembly.Location));
            container.RegisterType(typeof(ConnectionStringSettingsAspect), assembly.GetExportedTypes().Single(t => typeof(ConnectionStringSettingsAspect).IsAssignableFrom(t)));
            container.RegisterType<DataConnectionFactory>();
            container.RegisterType<SmoConnectionFactory>();

            var createDatabases = container.Resolve<CreateDatabasesCommand>();
            var dropDatabases = container.Resolve<DropDatabasesCommand>();
            var createSchemata = container.Resolve<CreateDatabaseSchemataCommand>();
            var runTests = container.Resolve<RunTestsCommand>();
            var validateSchemata = container.Resolve<ValidateDatabaseSchemataCommand>();

            dropDatabases.Execute();
            createDatabases.Execute();
            createSchemata.Execute();
            //validateSchemata.Execute();
            runTests.Execute();
        }
    }
}
