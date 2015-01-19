using System;
using System.Linq;

using NuClear.AdvancedSearch.EntityDataModel.Metadata.Features;

namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    public static class EntityRelationElementExtensions
    {
        public static string ResolveName(this EntityRelationElement relationElement)
        {
            if (relationElement == null)
            {
                throw new ArgumentNullException("relationElement");
            }

            return relationElement.Identity.ResolveName();
        }

        public static EntityRelationCardinality GetCardinality(this EntityRelationElement relationElement)
        {
            {
                var cardinalityFeature = relationElement.Features.OfType<EntityRelationCardinalityFeature>().SingleOrDefault();
                if (cardinalityFeature == null)
                {
                    throw new InvalidOperationException("The cardinality was not specified.");
                }

                return cardinalityFeature.Cardinality;
            }
        }

        public static EntityElement GetTarget(this EntityRelationElement relationElement)
        {
            var target = relationElement.Elements.OfType<EntityElement>().SingleOrDefault();
            if (target == null)
            {
                throw new InvalidOperationException("The target entity was not specified.");
            }

            return target;
        }
    }
}