using System;

using LinqToDB.Data;

using NuClear.Storage.API.Readings;

namespace NuClear.Replication.Bulk.Api.Storage
{
    public interface IStorage : IDisposable
    {
        IQuery GetReadAccess();
        DataConnection GetWriteAccess();
    }
}