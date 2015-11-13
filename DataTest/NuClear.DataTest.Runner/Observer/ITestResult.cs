using System;

namespace NuClear.DataTest.Runner.Observer
{
    public interface ITestResult
    {
        string Report { get; }
        Exception Asserted { get; }
        Exception Unhandled { get; }
        TestResultStatus Status { get; }
    }
}