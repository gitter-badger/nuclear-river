using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using NuClear.DataTest.Metamodel;

namespace NuClear.DataTest.Runner.Comparer
{
    public sealed class MessageFormatter
    {
        private readonly Func<object, string> _serializer;

        public MessageFormatter(Type entityType, SchemaMetadataElement schemaMetadataElement)
        {
            _serializer = CreateSerializer(entityType, schemaMetadataElement);
        }

        public IEnumerable<string> Format(CompareResult result)
        {
            foreach (var serializedEntity in result.Unexpected.Select(_serializer))
            {
                yield return $"unexpected: {serializedEntity}";
            }

            foreach (var serializedEntity in result.Missing.Select(_serializer))
            {
                yield return $"missing: {serializedEntity}";
            }

            foreach (var serializedEntity in result.Wrong.Select(tuple => Tuple.Create(_serializer.Invoke(tuple.Item1), _serializer.Invoke(tuple.Item2))))
            {
                Debug.Indent();
                yield return $"wrong: actual: {serializedEntity.Item1}, expected: {serializedEntity.Item2}";
            }
        }

        private Func<object, string> CreateSerializer(Type entityType, SchemaMetadataElement schemaMetadataElement)
        {
            var properties = schemaMetadataElement.Schema.GetEntityDescriptor(entityType).Columns.OrderBy(x => !x.IsPrimaryKey).Select(x => x.MemberAccessor);
            return obj => string.Concat(
                obj.GetType().Name,
                "{",
                string.Join(", ", properties.Select(property => $"{property.Name}: '{property.GetValue(obj)}'")),
                "}");
        }
    }
}