using System;

namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    public static class BoundedContextElementExtensions
    {
        public static string ResolveNamespace(this BoundedContextElement contextElement)
        {
            if (contextElement == null)
            {
                throw new ArgumentNullException("contextElement");
            }

            return contextElement.Identity.ResolveFullName();
        }
    }
}