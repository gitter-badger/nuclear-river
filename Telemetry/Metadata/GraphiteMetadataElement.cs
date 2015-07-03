namespace NuClear.Telemetry
{
    // TODO {a.rechkalov, 22.06.2015}: MetadataElement из ERM
    public class GraphiteMetadataElement
    {
        public GraphiteMetadataElement(string name, CounterType type)
        {
            Type = type;
            Name = name;
        }

        public string Name { get; private set; }
        public CounterType Type { get; private set; }

        public enum CounterType
        {
            Gauge, Counter, Timer
        }
    }
}