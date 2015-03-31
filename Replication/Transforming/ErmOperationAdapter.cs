using System;
using System.Collections.Generic;

using NuClear.AdvancedSearch.Replication.Model.CustomerIntelligence;

namespace NuClear.AdvancedSearch.Replication.Transforming
{
    using ErmEntityType = System.Int32;
    using ErmEntityId = System.Int64;
    using ErmOperation = System.Int32;

    public sealed class ErmOperationAdapter
    {
        private static readonly Dictionary<int, Type> EntityTypeMap = new Dictionary<int, Type>
                                                                      {
                                                                          { Code.Account, typeof(Fact.Account) },
                                                                          { Code.CategoryFirmAddress, typeof(Fact.CategoryFirmAddress) },
                                                                          { Code.CategoryOrganizationUnit, typeof(Fact.CategoryOrganizationUnit) },
                                                                          { Code.Client, typeof(Fact.Client) },
                                                                          { Code.Contact, typeof(Fact.Contact) },
                                                                          { Code.Firm, typeof(Fact.Firm) },
                                                                          { Code.FirmAddress, typeof(Fact.FirmAddress) },
                                                                          { Code.FirmContact, typeof(Fact.FirmContact) },
                                                                          { Code.LegalPerson, typeof(Fact.LegalPerson) },
                                                                          { Code.Order, typeof(Fact.Order) }
                                                                      };

        public IEnumerable<OperationInfo> Convert(IEnumerable<Tuple<ErmOperation, ErmEntityType, ErmEntityId>> changes)
        {
            foreach (var change in changes)
            {
                Type entityType;
                if (!EntityTypeMap.TryGetValue(change.Item2, out entityType))
                {
                    // exception
                    continue;
                }
                
                var operation = (Operation)change.Item1;
                var entityId = change.Item3;

                yield return new OperationInfo(operation, entityType, entityId);
            }
        }

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