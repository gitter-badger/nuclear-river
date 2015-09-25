using NuClear.AdvancedSearch.Common.Settings;
using NuClear.Settings.API;

namespace NuClear.Querying.Web.OData.Settings
{
    public sealed class WebApplicationSettings : SettingsContainerBase
    {
        public WebApplicationSettings()
        {
            Aspects.Use<EnvironmentSettingsAspect>()
                   .Use<ConnectionStringsSettingsAspect>();
        }
    }
}