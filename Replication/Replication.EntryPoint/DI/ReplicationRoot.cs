using NuClear.Assembling.Zones;

namespace NuClear.Replication.EntryPoint.DI
{
    internal static class ReplicationRoot
    {
        public static CompositionRoot Instance
        {
            get
            {
                return CompositionRoot.Config
                                      .RequireZone<ReplicationZone>()
                                      .UseAnchor<ReplicationEntryPointAssembly>();
            }
        }
    }
}