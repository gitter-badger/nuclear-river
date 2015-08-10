using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;
using NuClear.Replication.OperationsProcessing.Metadata.Operations;

namespace NuClear.Replication.OperationsProcessing.Transports
{
    public static class StatisticsOperationExtensions
    {
        private static readonly Dictionary<Guid, Type> OperationIdRegistry =
            new Dictionary<Guid, Type>
            {
                { StatisticsOperationIdentity.Instance.Guid, typeof(CalculateStatisticsOperation) }
            };

        private static readonly Dictionary<Type, Guid> OperationTypeRegistry =
            OperationIdRegistry.ToDictionary(x => x.Value, x => x.Key);

        public static CalculateStatisticsOperation DeserializeStatisticsOperation(this XElement context)
        {
            return new CalculateStatisticsOperation
            {
                ProjectId = (long)context.Attribute("Project"),
                CategoryId = (long?)context.Attribute("Category"),
            };
        }

        public static XElement Serialize(this CalculateStatisticsOperation operation)
        {
            return new XElement("RecalulateStatistics",
                                new XAttribute("Project", operation.ProjectId),
                                operation.CategoryId.HasValue ? new XAttribute("Category", operation.CategoryId) : null);
        }

        public static Guid GetIdentity(this CalculateStatisticsOperation operation)
        {
            Guid guid;
            if (OperationTypeRegistry.TryGetValue(operation.GetType(), out guid))
            {
                return guid;
            }

            throw new ArgumentException(string.Format("Unknown operation type {0}", operation.GetType().Name), "operationType");
        }
    }
}