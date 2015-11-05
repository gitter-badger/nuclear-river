using System;
using System.Diagnostics;

using LinqToDB.Data;

using NUnit.Framework;

namespace NuClear.CustomerIntelligence.Replication.Tests.BulkLoading
{
    public class BulkLoadingFixtureBase
    {
        static BulkLoadingFixtureBase()
        {
#if DEBUG
            //LinqToDB.Common.Configuration.Linq.GenerateExpressionTest = true;
            DataConnection.TurnTraceSwitchOn(TraceLevel.Verbose);
            DataConnection.WriteTraceLine = (s1, s2) => Debug.WriteLine(s1, s2);
#endif
        }

        protected readonly ILoader _loader;

        public BulkLoadingFixtureBase(ILoader loader)
        {
            _loader = loader;
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            var disposableLoader = _loader as IDisposable;
            disposableLoader?.Dispose();
        }
    }
}