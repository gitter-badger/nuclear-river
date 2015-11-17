using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

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
            container.RegisterInstance<CommandlineParameters>(ParceCommandline(args));
            if (args.Contains("--teamcity"))
            {
                container.RegisterType<ITestStatusObserver, TeamCityTestStatusObserver>();
            }
            else
            {
                container.RegisterType<ITestStatusObserver, ConsoleTestStatusObserver>();
            }

            var createDatabases = container.Resolve<CreateDatabasesCommand>();
            var dropDatabases = container.Resolve<DropDatabasesCommand>();
            var createSchemata = container.Resolve<CreateDatabaseSchemataCommand>();
            var runTests = container.Resolve<RunTestsCommand>();
            var validateSchemata = container.Resolve<ValidateDatabaseSchemataCommand>();

            //dropDatabases.Execute();
            //createDatabases.Execute();
            //createSchemata.Execute();
            //validateSchemata.Execute();
            runTests.Execute();

            return runTests.AnyFailedTest ? -1 : 0;
        }

        private static CommandlineParameters ParceCommandline(string[] args)
        {
            // --key1=value1 --key2 = value two
            var exp = new Regex(@"(--(?'key'.+?)=(?'value'.+?)(($)|(?=--)))");
            var matches = exp.Matches(string.Join(" ", args));
            var arguments = matches.Cast<Match>().ToDictionary(x => x.Groups["key"].Value.Trim(), x => x.Groups["value"].Value.Trim());
            return new CommandlineParameters(arguments);
        }
    }
}
