using NuClear.AdvancedSearch.Settings;
using NuClear.Settings.API;

namespace NuClear.AdvancedSearch.Web.OData.Settings
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