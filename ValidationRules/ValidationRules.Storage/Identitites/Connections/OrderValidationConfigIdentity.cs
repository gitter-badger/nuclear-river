using NuClear.Model.Common;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.ValidationRules.Storage.Identitites.Connections
{
    public class OrderValidationConfigIdentity : IdentityBase<OrderValidationConfigIdentity>, IConnectionStringIdentity
    {
        public override int Id
        {
            get { return 16; }
        }

        public override string Description
        {
            get { return "Path to orderValidation.config file"; }
        }
    }
}