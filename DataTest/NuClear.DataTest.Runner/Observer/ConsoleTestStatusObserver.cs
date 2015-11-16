using System;

using NuClear.DataTest.Metamodel.Dsl;

namespace NuClear.DataTest.Runner.Observer
{
    public sealed class ConsoleTestStatusObserver : ITestStatusObserver
    {
        public void Started(TestCaseMetadataElement test)
        {
        }

        public void Asserted(TestCaseMetadataElement test, Exception exception)
        {
            Console.WriteLine("Fail: " + test.Identity.Id);
            Console.WriteLine(exception.Message);
        }

        public void Succeeded(TestCaseMetadataElement test)
        {
            Console.WriteLine("Ok: " + test.Identity.Id);
        }
    }
}