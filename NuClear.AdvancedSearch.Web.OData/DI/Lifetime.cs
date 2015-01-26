using Microsoft.Practices.Unity;

// ReSharper disable once CheckNamespace
namespace DoubleGis.Erm.Platform.DI.Common.Config
{
    public static class Lifetime
    {
        public static LifetimeManager Singleton
        {
            get { return new ContainerControlledLifetimeManager(); }
        }

        public static LifetimeManager PerResolve
        {
            get { return new PerResolveLifetimeManager(); }
        }

        public static LifetimeManager PerThread
        {
            get { return new PerThreadLifetimeManager(); }
        }

        public static LifetimeManager Hierarchical
        {
            get { return new HierarchicalLifetimeManager(); }
        }

        /// <summary>
        /// Общая идея данной реализации per scope lifetime behaviour (время жизни ограниченно опеределнным scope - один using, один вызов метода и т.д.) 
        /// использование HierarchicalLifetimeManager:
        /// - все регистрации всех типов делаются как и раньше в головном контейнере
        /// - те типы которым нужно perscope lifetime поведение регистрируются с использованием HierarchicalLifetimeManager
        /// - на каждый scope создается свой instance child контейнера, через CreateChildContainer
        /// - каждый scope лучше оборачивать в using чтобы обязательно вызвать dispose childcontainer
        /// - disposableextension остается необходим, только если в головном контейнере, есть регистрации с perresolve lifetime - чтобы обеспечить их dispose
        /// - для повышения кармы - можно допилить disposableextension - чтобы он не вызывал dispose у экземпляров типов которые зарегистрированы с ContainerControlledLifetimeManager, HierarchicalLifetimeManager
        /// Важное замечание - dispose для конкретных экземпляров может вызываться несколько раз, т.к. сначала его вызывает сам child контейнер, при dispose самого контейнера, 
        /// а потом его вызывает disposableextension, также из-за dispose контейнера - друг про друга эти товарищи не знают => могут вызывать dispose несколько раз.
        /// Т.к. для реализации IDisposable в MSDN указано, что Dispose может вызываться несколько раз, то такой множественый вызов можно считать пусть и некрасивым, но корректным.
        /// </summary>
        public static LifetimeManager PerScope
        {
            get { return new HierarchicalLifetimeManager(); }
        }

        public static LifetimeManager External
        {
            get { return new ExternallyControlledLifetimeManager(); }
        }

        public static LifetimeManager Transient
        {
            get { return new TransientLifetimeManager(); }
        }
    }
}
