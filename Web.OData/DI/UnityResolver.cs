using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dependencies;

using Microsoft.Practices.Unity;

namespace NuClear.AdvancedSearch.Web.OData.DI
{
    internal sealed class UnityResolver : IDependencyResolver
    {
        private readonly IUnityContainer _container;

        public UnityResolver(IUnityContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            _container = container;
        }

        public static void RegisterHttpRequestMessage(HttpConfiguration config)
        {
            config.MessageHandlers.Add(new HttpRequestMessageRegistrator());
        }

        public object GetService(Type serviceType)
        {
            try
            {
                return _container.Resolve(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return _container.ResolveAll(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return new List<object>();
            }
        }

        public IDependencyScope BeginScope()
        {
            var child = _container.CreateChildContainer();
            return new UnityResolver(child);
        }

        public void Dispose()
        {
            _container.Dispose();
        }

        private sealed class HttpRequestMessageRegistrator : DelegatingHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var resolver = (UnityResolver)request.GetDependencyScope();
                resolver._container.RegisterInstance(request);
                return base.SendAsync(request, cancellationToken);
            }
        }
    }
}