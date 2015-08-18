using System;

using NuClear.AdvancedSearch.Replication.API.Transforming;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.Storage.Core;
using NuClear.Storage.LinqToDB;
using NuClear.Storage.Readings;
using NuClear.Storage.Writings;

namespace NuClear.AdvancedSearch.Replication.Tests
{
    public class StubFactChangesApplierFactory : IFactChangesApplierFactory
    {
        private readonly IModifiableDomainContextProvider _modifiableDomainContextProvider;

        public StubFactChangesApplierFactory(IModifiableDomainContextProvider modifiableDomainContextProvider)
        {
            _modifiableDomainContextProvider = modifiableDomainContextProvider;
        }

        public ISourceChangesApplier Create(IFactInfo factInfo, IQuery sourceQuery)
        {
            var repositoryType = typeof(LinqToDBRepository<>).MakeGenericType(factInfo.Type);
            var repositoryInstance = (IRepository)Activator.CreateInstance(repositoryType, _modifiableDomainContextProvider);

            var applierType = typeof(FactChangesApplier<>).MakeGenericType(factInfo.Type);
            return (ISourceChangesApplier)Activator.CreateInstance(applierType, factInfo, sourceQuery, repositoryInstance);
        }
    }
}