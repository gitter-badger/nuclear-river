using System;

using NuClear.DataTest.Metamodel.Dsl;

namespace NuClear.DataTest.Runner.Observer
{
    public interface ITestStatusObserver
    {
        void Started(TestCaseMetadataElement test);
        void Asserted(TestCaseMetadataElement test, Exception exception);
        void Succeeded(TestCaseMetadataElement test);
    }
}