using NuClear.Assembling.Zones;
using NuClear.Jobs;

namespace NuClear.AdvancedSearch.Replication.EntryPoint.DI
{
    public class ReplicationEntryPointAssembly : IZoneAssembly<ReplicationZone>,
                                                 IZoneAnchor<ReplicationZone>,
                                                 IContainsType<ITaskServiceJob>
    {
    }
}