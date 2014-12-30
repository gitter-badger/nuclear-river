using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;

namespace EntityDataModel.EntityFramework.Tests
{
    internal static class EdmModelExtensions
    {
        public static EntityType FindDeclaredType(this EdmModel model, string entityName)
        {
            return model.EntityTypes.FirstOrDefault(x => x.FullName == entityName);
        }

        public static EntitySet FindEntitySet(this EntityContainer container, string entityName)
        {
            return container.EntitySets.FirstOrDefault(x => x.Name == entityName);
        }

        public static EdmProperty FindProperty(this EntityType entityType, string entityName)
        {
            return entityType.Properties.FirstOrDefault(x => x.Name == entityName);
        }
    }
}