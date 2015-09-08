using System;

using NuClear.AdvancedSearch.Replication.API.Transforming;
using NuClear.AdvancedSearch.Replication.API.Transforming.Facts;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.Storage.Core;
using NuClear.Storage.LinqToDB;
using NuClear.Storage.Writings;

namespace NuClear.AdvancedSearch.Replication.Tests
{
    public class StubDataChangesApplierFactory : IDataChangesApplierFactory
    {
        private readonly IModifiableDomainContextProvider _modifiableDomainContextProvider;

        public StubDataChangesApplierFactory(IModifiableDomainContextProvider modifiableDomainContextProvider)
        {
            _modifiableDomainContextProvider = modifiableDomainContextProvider;
        }

        public IDataChangesApplier Create(Type type)
        {
            var repositoryType = typeof(LinqToDBRepository<>).MakeGenericType(type);
            var repositoryInstance = (IRepository)Activator.CreateInstance(repositoryType, _modifiableDomainContextProvider);

            var applierType = typeof(DataChangesApplier<>).MakeGenericType(type);
            return (IDataChangesApplier)Activator.CreateInstance(applierType, repositoryInstance);
        }
    }
}