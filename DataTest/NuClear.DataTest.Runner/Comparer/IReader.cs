using System.Collections.Generic;

namespace NuClear.DataTest.Runner.Comparer
{
    public interface IReader
    {
        IReadOnlyCollection<T> Read<T>() where T : class;
    }
}