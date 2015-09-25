using NuClear.IdentityService.Client.Interaction;

namespace NuClear.Replication.OperationsProcessing.Transports.SQLStore
{
    public class IdentityGenerator : IIdentityGenerator
    {
        private readonly IIdentityServiceClient _identityServiceClient;

        public IdentityGenerator(IIdentityServiceClient identityServiceClient)
        {
            _identityServiceClient = identityServiceClient;
        }

        public long Next()
        {
            return _identityServiceClient.GetIdentities(1)[0];
        }
    }
}