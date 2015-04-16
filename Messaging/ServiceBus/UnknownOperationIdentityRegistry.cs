using System;

using NuClear.Model.Common.Operations.Identity;

namespace NuClear.AdvancedSearch.Messaging.ServiceBus
{
    public sealed class UnknownOperationIdentityRegistry : IOperationIdentityRegistry
    {
        public TOperationIdentity GetIdentity<TOperationIdentity>() where TOperationIdentity : IOperationIdentity
        {
            throw new NotImplementedException();
        }

        public IOperationIdentity GetIdentity(Type identityType)
        {
            throw new NotImplementedException();
        }

        public IOperationIdentity GetIdentity(int operationId)
        {
            return new UnknownOperationIdentity().SetId(operationId);
        }

        public IOperationIdentity[] Identities
        {
            get { throw new NotImplementedException(); }
        }
    }
}