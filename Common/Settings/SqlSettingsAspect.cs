using NuClear.Settings;
using NuClear.Settings.API;

namespace NuClear.AdvancedSearch.Common.Settings
{
    public sealed class SqlSettingsAspect : ISettingsAspect, ISqlSettingsAspect
    {
        private readonly IntSetting _sqlCommandTimeout = ConfigFileSetting.Int.Required("SqlCommandTimeout");

        public int SqlCommandTimeout
        {
            get { return _sqlCommandTimeout.Value; }
        }
    }
}
