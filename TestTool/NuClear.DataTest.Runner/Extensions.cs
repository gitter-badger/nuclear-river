using System;
using System.Configuration;
using System.Linq;
using System.Reflection;

using NuClear.Metamodeling.Provider.Sources;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.DataTest.Runner
{
    static class Extensions
    {
        public static ConnectionStringSettingsAspect GetConnectionStrings(this Assembly assembly)
        {
            try
            {
                var type = assembly.GetExportedTypes().SingleOrDefault(t => typeof(ConnectionStringSettingsAspect).IsAssignableFrom(t));
                if (type == null)
                {
                    throw new Exception("Settings not found");
                }

                var testConfigurationFile = ConfigurationManager.OpenExeConfiguration(assembly.Location);
                return (ConnectionStringSettingsAspect)Activator.CreateInstance(type, testConfigurationFile);
            }
            catch (Exception ex)
            {
                throw new Exception($"Can not load ConnectionStringSettingsAspect from {assembly.FullName}", ex);
            }
        }

        public static IMetadataSource[] GetMetadataSources(this Assembly assembly)
        {
            try
            {
                return assembly.GetExportedTypes()
                                    .Where(t => typeof(IMetadataSource).IsAssignableFrom(t))
                                    .Select(type => Activator.CreateInstance(type, new object[0]))
                                    .Cast<IMetadataSource>()
                                    .ToArray();
            }
            catch (Exception ex)
            {
                throw new Exception($"Can not load metadata sources from {assembly.FullName}", ex);
            }
        }
    }
}
