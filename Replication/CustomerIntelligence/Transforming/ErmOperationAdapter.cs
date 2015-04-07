using System;
using System.Collections.Generic;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts;
using NuClear.AdvancedSearch.Replication.Transforming;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    using ErmEntityType = System.Int32;
    using ErmEntityId = System.Int64;
    using ErmOperation = System.Int32;

    public sealed class ErmOperationAdapter
    {
        private static readonly Dictionary<int, Type> EntityTypeMap = new Dictionary<int, Type>
                                                                      {
                                                                          { Code.Account, typeof(Account) },
                                                                          { Code.CategoryFirmAddress, typeof(CategoryFirmAddress) },
                                                                          { Code.CategoryOrganizationUnit, typeof(CategoryOrganizationUnit) },
                                                                          { Code.Client, typeof(Client) },
                                                                          { Code.Contact, typeof(Contact) },
                                                                          { Code.Firm, typeof(Firm) },
                                                                          { Code.FirmAddress, typeof(FirmAddress) },
                                                                          { Code.FirmContact, typeof(FirmContact) },
                                                                          { Code.LegalPerson, typeof(LegalPerson) },
                                                                          { Code.Order, typeof(Order) }
                                                                      };

//        public IEnumerable<OperationInfo> Convert(IEnumerable<Tuple<ErmOperation, ErmEntityType, ErmEntityId>> changes)
//        {
//            foreach (var change in changes)
//            {
//                Type entityType;
//                if (!EntityTypeMap.TryGetValue(change.Item2, out entityType))
//                {
//                    // exception
//                    continue;
//                }
//                
//                var operation = (Operation)change.Item1;
//                var entityId = change.Item3;
//
//                yield return new OperationInfo(operation, entityType, entityId);
//            }
//        }
//
        private static class Code
        {
            public const int Account = 142;
            public const int CategoryFirmAddress = 166;
            public const int CategoryOrganizationUnit = 161;
            public const int Client = 200;
            public const int Contact = 206;
            public const int Firm = 146;
            public const int FirmAddress = 164;
            public const int FirmContact = 165;
            public const int LegalPerson = 147;
            public const int Order = 151;
        }
    }
}