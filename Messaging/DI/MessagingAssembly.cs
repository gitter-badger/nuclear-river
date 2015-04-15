using NuClear.Assembling.Zones;
using NuClear.Metamodeling.Provider.Sources;

namespace NuClear.AdvancedSearch.Messaging.DI
{
    public class MessagingAssembly : IZoneAssembly<MetadataZone>,
                                     IZoneAnchor<MetadataZone>,
                                     IContainsType<IMetadataSource>
    {
    }
}