using System;
using System.Linq;

using Moq;

namespace NuClear.CustomerIntelligence.Replication.Tests
{
    public class DynamicMock
    {
        public static object Of(Type type)
        {
            var mock = typeof(Mock<>).MakeGenericType(type).GetConstructor(Type.EmptyTypes).Invoke(new object[] { });
            return mock.GetType().GetProperties().Single(f => f.Name == "Object" && f.PropertyType == type).GetValue(mock, new object[] { });
        }
    }
}