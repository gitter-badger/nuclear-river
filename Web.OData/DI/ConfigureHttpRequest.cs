using System.Net.Http;

using Microsoft.Practices.Unity;

namespace NuClear.AdvancedSearch.Web.OData.DI
{
    // alias for Func<IUnityContainer, HttpRequestMessage, IUnityContainer>
    public delegate IUnityContainer ConfigureHttpRequest(IUnityContainer container, HttpRequestMessage request);
}