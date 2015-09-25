using NuClear.Assembling.Zones;

namespace NuClear.AdvancedSearch.Replication.EntryPoint.DI
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