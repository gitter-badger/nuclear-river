using System;
using System.Diagnostics;

using LinqToDB.Data;

namespace NuClear.CustomerIntelligence.Replication.Tests.Data
{
    internal sealed class NoSqlTrace : IDisposable
    {
        private readonly TraceLevel _level;

        public NoSqlTrace(TraceLevel level = TraceLevel.Off)
        {
            _level = DataConnection.TraceSwitch.Level;
            DataConnection.TurnTraceSwitchOn(level);
        }

        public void Dispose()
        {
            DataConnection.TurnTraceSwitchOn(_level);
        }
    }
}