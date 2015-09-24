using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using NuClear.Replication.Metadata.Operations;
using NuClear.Replication.OperationsProcessing.Identities.Operations;

namespace NuClear.CustomerIntelligence.OperationsProcessing
{
    public static class StatisticsOperationExtensions
    {
        private static readonly Dictionary<Guid, Type> OperationIdRegistry =
            new Dictionary<Guid, Type>
            {
                { StatisticsOperationIdentity.Instance.Guid, typeof(RecalculateStatisticsOperation) }
            };

        private static readonly Dictionary<Type, Guid> OperationTypeRegistry =
            OperationIdRegistry.ToDictionary(x => x.Value, x => x.Key);

        public static RecalculateStatisticsOperation DeserializeStatisticsOperation(this XElement context)
        {
            return new RecalculateStatisticsOperation
                   {
                       ProjectId = (long)context.Attribute("Project"),
                       CategoryId = (long?)context.Attribute("Category"),
                   };
        }

        public static XElement Serialize(this RecalculateStatisticsOperation operation)
        {
            return new XElement("RecalulateStatistics",
                                new XAttribute("Project", operation.ProjectId),
                                operation.CategoryId.HasValue ? new XAttribute("Category", operation.CategoryId) : null);
        }

        public static Guid GetIdentity(this RecalculateStatisticsOperation operation)
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