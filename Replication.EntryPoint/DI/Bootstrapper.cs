using Microsoft.Practices.Unity;

using NuClear.Assembling.TypeProcessing;
using NuClear.DI.Unity.Config;
using NuClear.Jobs.Schedulers;
using NuClear.Jobs.Unity;
using NuClear.Settings.API;
using NuClear.Settings.Unity;
using NuClear.Tracing.API;

using Quartz.Spi;

namespace NuClear.AdvancedSearch.Replication.EntryPoint.DI
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
            container.ConfigureSettingsAspects(settingsContainer)
                     .ConfigureTracing(tracer, tracerContextManager)
                     .ConfigureQuartz()
                     .PerformTypeRegistrations();

            ReplicationRoot.Instance.PerformTypesMassProcessing(massProcessors, true, typeof(object));

            return container;
        }

        public static IUnityContainer ConfigureTracing(this IUnityContainer container, ITracer tracer, ITracerContextManager tracerContextManager)
        {
            return container.RegisterInstance(tracer)
                            .RegisterInstance(tracerContextManager);
        }

        private static IUnityContainer ConfigureQuartz(this IUnityContainer container)
        {
            return container
                .RegisterType<IJobFactory, JobFactory>(Lifetime.Singleton, new InjectionFactory(c => c.Resolve<UnityJobFactory>()))
                .RegisterType<IJobStoreFactory, JobStoreFactory>(Lifetime.Singleton)
                .RegisterType<ISchedulerManager, SchedulerManager>(Lifetime.Singleton);
        }

        private static IUnityContainer PerformTypeRegistrations(this IUnityContainer container)
        {
            // TODO: Add type registrations here
            return container;
        }
    }
}