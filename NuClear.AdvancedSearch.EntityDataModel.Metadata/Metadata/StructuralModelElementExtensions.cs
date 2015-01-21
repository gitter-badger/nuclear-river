using System.Collections.Generic;
using System.Linq;

namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    public static class StructuralModelElementExtensions
    {
        public static IEnumerable<EntityElement> GetFlattenEntities(this StructuralModelElement element)
        {
            foreach (var entityElement in element.Entities)
            {
                foreach (var relatedEntityElement in EnumerateRelatedElements(entityElement))
                {
                    yield return relatedEntityElement;
                }
                yield return entityElement;
            }
        }

        private static IEnumerable<EntityElement> EnumerateRelatedElements(EntityElement entityElement)
        {
            foreach (var relatedElement in entityElement.GetRelations().Select(x => x.GetTarget()))
            {
                foreach (var nestedRelatedElement in EnumerateRelatedElements(relatedElement))
                {
                    yield return nestedRelatedElement;
                }
                yield return relatedElement;
            }
        }
   }
}