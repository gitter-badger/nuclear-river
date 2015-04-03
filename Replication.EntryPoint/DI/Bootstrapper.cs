using Microsoft.Practices.Unity;

using NuClear.Assembling.TypeProcessing;
using NuClear.DI.Unity.Config;
using NuClear.Jobs.DI;
using NuClear.Jobs.Schedulers;
using NuClear.Settings.API;
using NuClear.Settings.Unity;
using NuClear.Tracing.API;

using Quartz.Spi;

namespace Replication.EntryPoint.DI
{
    public static class Bootstrapper
    {
        public static IUnityContainer ConfigureUnity(ISettingsContainer settingsContainer, ITracer tracer, ITracerContextManager tracerContextManager)
        {
            IUnityContainer container = new UnityContainer();
            var massProcessors = new IMassProcessor[]
                                 {
                                     new TaskServiceJobsMassProcessor(container) 
                                 };
            container.ConfigureSettingsAspects(settingsContainer);
            container.PerformRegistrations();
            ReplicationRoot.Instance.PerformTypesMassProcessing(massProcessors, true, typeof(object));

            return container;
        }

        private static IUnityContainer ConfigureQuartz(this IUnityContainer container)
        {
            return container
                .RegisterType<IJobFactory, JobFactory>(Lifetime.Singleton)
                .RegisterType<ISchedulerManager, SchedulerManager>(Lifetime.Singleton);
        }

        private static IUnityContainer PerformRegistrations(this IUnityContainer container)
        {
            // TODO: Add type registrations here
            return container;
        }
    }
}