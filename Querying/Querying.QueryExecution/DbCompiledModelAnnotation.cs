using System.Data.Entity.Infrastructure;

namespace NuClear.Querying.QueryExecution
{
    public sealed class DbCompiledModelAnnotation
    {
        public DbCompiledModelAnnotation(DbCompiledModel value)
        {
            Value = value;
        }

        public DbCompiledModel Value { get; set; }
    }
}