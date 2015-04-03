using NuClear.Assembling.Zones;
using NuClear.Jobs;

namespace Replication.EntryPoint.DI
{
    public class ReplicationEntryPointAssembly : IZoneAssembly<ReplicationZone>,
                                                 IZoneAnchor<ReplicationZone>,
                                                 IContainsType<ITaskServiceJob>
    {
    }
}