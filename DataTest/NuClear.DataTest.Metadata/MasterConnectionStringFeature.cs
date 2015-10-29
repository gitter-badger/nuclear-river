using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.DataTest.Metamodel
{
    public sealed class MasterConnectionStringFeature : IUniqueMetadataFeature
    {
        public MasterConnectionStringFeature(IConnectionStringIdentity connectionStringIdentity)
        {
            ConnectionStringIdentity = connectionStringIdentity;
        }

        public IConnectionStringIdentity ConnectionStringIdentity { get; }
    }
}