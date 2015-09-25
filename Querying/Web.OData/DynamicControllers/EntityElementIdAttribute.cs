using System;

namespace NuClear.AdvancedSearch.Web.OData.DynamicControllers
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class EntityElementIdAttribute : Attribute
    {
        public EntityElementIdAttribute(string uri)
        {
            Uri = new Uri(uri);
        }

        public Uri Uri { get; private set; }
    }
}