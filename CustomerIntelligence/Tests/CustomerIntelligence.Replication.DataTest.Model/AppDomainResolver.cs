using System;
using System.Collections.Generic;
using System.IO;

using NuClear.DataTest.Metamodel.Dsl;

namespace CustomerIntelligence.Replication.DataTest.Model
{
    internal sealed class AppDomainResolver
    {
        private static readonly IDictionary<ActMetadataElement, AppDomain> AppDomainCache = new Dictionary<ActMetadataElement, AppDomain>();

        public AppDomain GetDomainFor(ActMetadataElement metadataElement, FileInfo exe)
        {
            AppDomain domain;
            if (!AppDomainCache.TryGetValue(metadataElement, out domain))
            {
                domain = AppDomain.CreateDomain($"bulk-tool-{metadataElement.Identity.Id}", null, exe.DirectoryName, "", false);
                AppDomainCache.Add(metadataElement, domain);
            }

            return domain;
        }
    }
}