namespace NuClear.CustomerIntelligence.Replication.StateInitialization.Tests
{
    public interface IKey
    {
        string Key { get; }
    }

    public sealed class Facts : IKey
    {
        public string Key => "-fact";
    }

    public sealed class CustomerIntelligence : IKey
    {
        public string Key => "-ci";
    }

    public sealed class Statistics : IKey
    {
        public string Key => "-statistics";
    }
}