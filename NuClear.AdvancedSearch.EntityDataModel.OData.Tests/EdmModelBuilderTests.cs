using System;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Validation;

using Moq;

using NuClear.AdvancedSearch.EntityDataModel.OData.Building;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace NuClear.AdvancedSearch.EntityDataModel.OData.Tests
{
    [TestFixture]
    internal class EdmModelBuilderTests
    {
        private Mock<IEdmModelSource> _modelSource;
        private EdmModelBuilder _modelBuilder;

        [SetUp]
        public void Setup()
        {
            _modelSource = new Mock<IEdmModelSource>();
            _modelBuilder = new EdmModelBuilder(_modelSource.Object);
        }

        [Test, Ignore]
        public void ShouldBuildValidModel()
        {
            var model = _modelBuilder.Build();
            Debug.WriteLine(model.Dump());

            Assert.NotNull(model);
            Assert.That(model, Model.IsValid);
        }

        [Test]
        public void ShouldBuildModel()
        {
            _modelSource.Setup(x => x.Namespace).Returns("CustomerIntelligence");

            var model = _modelBuilder.Build();
            Debug.WriteLine(model.Dump());

            Assert.NotNull(model);
            Assert.That(model, Model.IsValid);
        }

        #region Model constraints

        private static class Model
        {
            public static Constraint IsValid
            {
                get
                {
                    return new ModelValidationConstraint();
                }
            }

            private class ModelValidationConstraint : Constraint
            {
                private const int MaxErrorsToDisplay = 5;
                private IEnumerable<EdmError> _errors;

                public override bool Matches(object actual)
                {
                    var model = actual as IEdmModel;
                    if (model == null)
                    {
                        throw new ArgumentException("The specified actual value is not a model.", "actual");
                    }

                    return model.Validate(out _errors);
                }

                public override void WriteDescriptionTo(MessageWriter writer)
                {
                    writer.WriteCollectionElements(_errors, 0, MaxErrorsToDisplay);
                }
            }
        }

        #endregion
    }
}
