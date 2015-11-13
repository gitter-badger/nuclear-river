using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.DataTest.Metamodel.Dsl;

namespace NuClear.DataTest.Runner.Observer
{
    public sealed class TeamCityTestStatusObserver : ITestStatusObserver
    {
        // Makes use of TeamCity's support for Service Messages
        // http://confluence.jetbrains.com/display/TCD8/Build+Script+Interaction+with+TeamCity#BuildScriptInteractionwithTeamCity-ReportingTests

        public void Started(TestCaseMetadataElement test)
        {
            Console.WriteLine(new TeamCityMessage("testStarted")
                                  .WithProperty("name", ResolveTestId(test)));
        }

        public void Asserted(TestCaseMetadataElement test, Exception exception)
        {
            Console.WriteLine(new TeamCityMessage("testFailed")
                                  .WithProperty("name", ResolveTestId(test))
                                  .WithProperty("message", "Asserted. ")
                                  .WithProperty("details", exception.Message)
                                  .WithProperty("expected", "Succeeded")
                                  .WithProperty("actual", "Failed"));

            FinishTest(test);
        }

        public void Succeeded(TestCaseMetadataElement test)
        {
            FinishTest(test);
        }

        private static void FinishTest(TestCaseMetadataElement test)
        {
            Console.WriteLine(new TeamCityMessage("testFinished")
                                  .WithProperty("name", ResolveTestId(test)));
        }

        private static string ResolveTestId(TestCaseMetadataElement test)
        {
            return test.Identity.Id.ToString();
        }

        private class TeamCityMessage
        {
            private const int MaxPropertyValueLength = 1000;

            private static readonly Func<string, string>[] PropertyValueNormalizers =
            {
                x => x ?? string.Empty,
                x => x.Replace("|", "||"), // должно быть первым
                x => x.Replace("\'", "|\'"),
                x => x.Replace("\n", "|n"),
                x => x.Replace("\r", "|r"),
                x => x.Replace("[", "|["),
                x => x.Replace("]", "|]"),
                x => x.Length > MaxPropertyValueLength ? x.Substring(0, MaxPropertyValueLength) + "..." : x
            };

            private readonly Dictionary<string, string> _properties = new Dictionary<string, string>();

            public TeamCityMessage(string header)
            {
                Header = header;
            }

            public string Header { get; private set; }

            public TeamCityMessage WithProperty(string name, string value)
            {
                value = PropertyValueNormalizers.Aggregate(value, (current, normalizer) => normalizer(current));

                _properties.Add(name, value);

                return this;
            }

            public override string ToString()
            {
                var properies = string.Join(" ", _properties.Select(x => string.Format("{0}='{1}'", x.Key, x.Value)));

                return string.Format("##teamcity[{0} {1}]", Header, properies);
            }
        }
    }
}