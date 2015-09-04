using System;

using NuClear.AdvancedSearch.Replication.API.Transforming.Facts;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.Storage.Core;
using NuClear.Storage.LinqToDB;
using NuClear.Storage.Readings;
using NuClear.Storage.Writings;

namespace NuClear.AdvancedSearch.Replication.Tests
{
    public class StubFactProcessorFactory : IFactProcessorFactory
    {
        private readonly IModifiableDomainContextProvider _modifiableDomainContextProvider;

        public StubFactProcessorFactory(IModifiableDomainContextProvider modifiableDomainContextProvider)
        {
            _modifiableDomainContextProvider = modifiableDomainContextProvider;
        }

        public IFactProcessor Create(IFactInfo factInfo, IQuery sourceQuery)
        {
            var repositoryType = typeof(LinqToDBRepository<>).MakeGenericType(factInfo.Type);
            var repositoryInstance = (IRepository)Activator.CreateInstance(repositoryType, _modifiableDomainContextProvider);

            var applierType = typeof(FactProcessor<>).MakeGenericType(factInfo.Type);
            return (IFactProcessor)Activator.CreateInstance(applierType, factInfo, sourceQuery, repositoryInstance);
        }
    }
}