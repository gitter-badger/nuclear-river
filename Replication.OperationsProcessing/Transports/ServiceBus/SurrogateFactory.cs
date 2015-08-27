using System;

namespace NuClear.Replication.OperationsProcessing.Transports.ServiceBus
{
    public static class SurrogateFactory<TSurrogate>
    {
        private static Func<TSurrogate> _factory;

        public static Func<TSurrogate> Factory
        {
            get { return EnsureStaticFactory; }
            set { _factory = value; }
        }

        private static TSurrogate EnsureStaticFactory()
        {
            return _factory();
        }
    }
}