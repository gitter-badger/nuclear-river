using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;

using Moq;

using NuClear.AdvancedSearch.EntityDataModel.Metadata;
using NuClear.EntityDataModel.EntityFramework.Building;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Processors;
using NuClear.Metamodeling.Provider;
using NuClear.Metamodeling.Provider.Sources;

using NUnit.Framework.Constraints;

namespace EntityDataModel.EntityFramework.Tests
{
    internal class EdmxBuilderBaseFixture
    {
        protected static DbModel BuildModel(BoundedContextElement context)
        {
            var model = EdmxModelBuilder.Build(ProcessContext(context));

            model.Dump();

            return model;
        }

        protected static EdmModel BuildConceptualModel(BoundedContextElement context)
        {
            var model = EdmxModelBuilder.Build(ProcessContext(context));

            model.ConceptualModel.Dump();

            return model.ConceptualModel;
        }

        protected static EdmModel BuildStoreModel(BoundedContextElement context)
        {
            var model = EdmxModelBuilder.Build(ProcessContext(context));

            //model.StoreModel.Dump();

            return model.StoreModel;
        }

        protected static BoundedContextElementBuilder NewContext(string name)
        {
            return BoundedContextElement.Config.Name(name);
        }

        protected static StructuralModelElementBuilder NewModel(params EntityElementBuilder[] entities)
        {
            var model = StructuralModelElement.Config;

            foreach (var entityElementBuilder in entities)
            {
                model.Elements(entityElementBuilder);
            }

            return model;
        }

        protected static EntityElementBuilder NewEntity(string name, params EntityPropertyElementBuilder[] properties)
        {
            var config = EntityElement.Config.Name(name);

            if (properties.Length == 0)
            {
                config.Property(NewProperty("Id").NotNull()).IdentifyBy("Id");
            }

            foreach (var propertyElementBuilder in properties)
            {
                config.Property(propertyElementBuilder);
            }

            return config;
        }

        protected static EntityPropertyElementBuilder NewProperty(string propertyName, EntityPropertyType propertyType = EntityPropertyType.Int64)
        {
            return EntityPropertyElement.Config.Name(propertyName).OfType(propertyType);
        }

        protected static EntityRelationElementBuilder NewRelation(string relationName)
        {
            return EntityRelationElement.Config.Name(relationName);
        }

        protected static class ConceptualModel
        {
            public static Constraint IsValid { get { return new ModelValidationConstraint(); } }

            private class ModelValidationConstraint : Constraint
            {
                private const int MaxErrorsToDisplay = 5;
                private IReadOnlyCollection<string> _errors;

                public override bool Matches(object value)
                {
                    var model = value as EdmModel;
                    if (model == null)
                    {
                        throw new ArgumentException("The specified actual value is not a model.", "value");
                    }

                    return model.IsValidCsdl(out _errors);
                }

                public override void WriteDescriptionTo(MessageWriter writer)
                {
                    writer.Write("valid");
                }

                public override void WriteActualValueTo(MessageWriter writer)
                {
                    if (_errors.Count == 0)
                    {
                        return;
                    }

                    writer.WriteLine("The model containing errors:");
                    foreach (var error in _errors.Take(MaxErrorsToDisplay))
                    {
                        writer.WriteMessageLine(2, error);
                    }
                }
            }
        }

        protected static class StoreModel
        {
            public static Constraint IsValid { get { return new ModelValidationConstraint(); } }

            private class ModelValidationConstraint : Constraint
            {
                private const int MaxErrorsToDisplay = 5;
                private IReadOnlyCollection<string> _errors;

                public override bool Matches(object value)
                {
                    var model = value as EdmModel;
                    if (model == null)
                    {
                        throw new ArgumentException("The specified actual value is not a model.", "value");
                    }

                    return model.IsValidSsdl(out _errors);
                }

                public override void WriteDescriptionTo(MessageWriter writer)
                {
                    writer.Write("valid");
                }

                public override void WriteActualValueTo(MessageWriter writer)
                {
                    if (_errors.Count == 0)
                    {
                        return;
                    }
                    
                    writer.WriteLine("The model containing errors:");
                    foreach (var error in _errors.Take(MaxErrorsToDisplay))
                    {
                        writer.WriteMessageLine(2, error);
                    }
                }
            }
        }

        protected static class Property
        {
            public static Predicate<EdmProperty> OfType(PrimitiveTypeKind typeKind)
            {
                return x => x.PrimitiveType.PrimitiveTypeKind == typeKind;
            }

            //            public static Predicate<IEdmProperty> OfKind(EdmPropertyKind propertyKind)
            //            {
            //                return x => x.PropertyKind == propertyKind;
            //            }
            //
            //            public static Predicate<IEdmProperty> IsCollection()
            //            {
            //                return x => x.Type.Definition is IEdmCollectionType;
            //            }

            public static Predicate<EdmProperty> Members(params string[] names)
            {
                return x => names.OrderBy(_ => _).SequenceEqual(x.EnumType.Members.Select(m => m.Name).OrderBy(_ => _));
            }
        }

        #region Utils

        private static IMetadataSource MockSource(IMetadataElement context)
        {
            var source = new Mock<IMetadataSource>();
            source.Setup(x => x.Kind).Returns(new AdvancedSearchIdentity());
            source.Setup(x => x.Metadata).Returns(new Dictionary<Uri, IMetadataElement> { { IdBuilder.For<AdvancedSearchIdentity>(), context } });

            return source.Object;
        }

        private static IMetadataProvider CreateProvider(params IMetadataSource[] sources)
        {
            return new MetadataProvider(sources, new IMetadataProcessor[0]);
        }

        private static BoundedContextElement ProcessContext(IMetadataElement context)
        {
            var provider = CreateProvider(MockSource(context));

            return provider.Metadata.Metadata.Values.OfType<BoundedContextElement>().FirstOrDefault();
        }

        #endregion
    }
}