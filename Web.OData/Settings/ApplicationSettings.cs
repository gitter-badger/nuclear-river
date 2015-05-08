using NuClear.Settings.API;

namespace NuClear.AdvancedSearch.Web.OData.Settings
{
    public sealed class ApplicationSettings : SettingsContainerBase
    {
        public ApplicationSettings()
        {
            Aspects.Use<TracerSettingsAspect>();
        }
    }
}