using System;
using System.Configuration;
using System.Linq;
using System.Reflection;

using Microsoft.Practices.Unity;

using NuClear.DataTest.Metamodel;
using NuClear.DataTest.Runner.Command;
using NuClear.DataTest.Runner.Observer;
using NuClear.Metamodeling.Processors;
using NuClear.Metamodeling.Provider;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.DataTest.Runner
{
    static class Program
    {
        public static int Main(string[] args)
        {
            var container = new UnityContainer();

            var assembly = Assembly.Load(AssemblyName.GetAssemblyName(args[0]));
            var metadataProvider = new MetadataProvider(assembly.GetExportedTypes()
                                                                .Where(t => typeof(IMetadataSource).IsAssignableFrom(t))
                                                                .Select(x => (IMetadataSource)container.Resolve(x))
                                                                .ToArray(),
                                                        new IMetadataProcessor[0]);

            container.RegisterInstance<IMetadataProvider>(metadataProvider);
            container.RegisterType(typeof(ConnectionStringSettingsAspect), assembly.GetExportedTypes().Single(t => typeof(ConnectionStringSettingsAspect).IsAssignableFrom(t)));
            container.RegisterType<DataConnectionFactory>();
            container.RegisterType<SmoConnectionFactory>();
#if DEBUG
            container.RegisterType<ITestStatusObserver, ConsoleTestStatusObserver>();
#else
            container.RegisterType<ITestStatusObserver, TeamCityTestStatusObserver>();
#endif
            var createDatabases = container.Resolve<CreateDatabasesCommand>();
            var dropDatabases = container.Resolve<DropDatabasesCommand>();
            var createSchemata = container.Resolve<CreateDatabaseSchemataCommand>();
            var runTests = container.Resolve<RunTestsCommand>();
            var validateSchemata = container.Resolve<ValidateDatabaseSchemataCommand>();

            dropDatabases.Execute();
            createDatabases.Execute();
            createSchemata.Execute();
            validateSchemata.Execute();
            runTests.Execute();

            return runTests.AnyFailedTest ? -1 : 0;
        }
    }
}
