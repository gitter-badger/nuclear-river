using System;
using System.Collections.Generic;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
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

        public IEnumerable<FactOperation> Convert(IEnumerable<ErmOperation> changes)
        {
            foreach (var change in changes)
            {
                Type entityType;
                if (!EntityTypeMap.TryGetValue(change.EntityType, out entityType))
                {
                    continue;
                }

                switch (change.Change)
                {
                    case OperationCode.Created:
                        yield return new CreateFact(entityType, change.EntityId);
                        break;
                    case OperationCode.Updated:
                        yield return new UpdateFact(entityType, change.EntityId);
                        break;
                    case OperationCode.Deleted:
                        yield return new DeleteFact(entityType, change.EntityId);
                        break;
                }
            }
        }

        public class ErmOperation
        {
            public int EntityType { get; set; }
            public long EntityId { get; set; }
            public int Change { get; set; }
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

        private static class OperationCode
        {
            public const int Created = 1;
            public const int Updated = 2;
            public const int Deleted = 3;
        }
    }
}