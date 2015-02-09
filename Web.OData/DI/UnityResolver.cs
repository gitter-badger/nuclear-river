using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
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

        public sealed class PerRequestResolver : DelegatingHandler
        {
            private readonly ConfigureHttpRequest _configureHttpRequest;

            public PerRequestResolver(ConfigureHttpRequest configureHttpRequest)
            {
                _configureHttpRequest = configureHttpRequest;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var scope = (UnityResolver)request.GetDependencyScope();
                _configureHttpRequest(scope._container, request);

                return base.SendAsync(request, cancellationToken);
            }
        }
    }
}