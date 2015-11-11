using System.Configuration;

namespace NuClear.CustomerIntelligence.Replication.Tests
{
    public static class Settings
    {
        public static int? SqlCommandTimeout
        {
            get
            {
                int timeout;
                var timeoutValue = ConfigurationManager.AppSettings["SqlCommandTimeout"];
                if (timeoutValue != null && int.TryParse(timeoutValue, out timeout))
                {
                    return timeout;
                }

                return null;
            }
        }
    }
}