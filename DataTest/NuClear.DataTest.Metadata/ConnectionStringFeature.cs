using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.DataTest.Metamodel
{
    public class ConnectionStringFeature : IUniqueMetadataFeature
    {
        public ConnectionStringFeature(IConnectionStringIdentity connectionStringIdentity)
        {
            ConnectionStringIdentity = connectionStringIdentity;
        }

        public IConnectionStringIdentity ConnectionStringIdentity { get; }
    }
}