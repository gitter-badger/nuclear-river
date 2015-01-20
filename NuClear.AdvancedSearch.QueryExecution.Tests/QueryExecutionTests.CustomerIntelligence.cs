using System.Data.Entity.Infrastructure;

using EntityDataModel.EntityFramework.Tests;

using Microsoft.OData.Edm;

using NuClear.AdvancedSearch.EntityDataModel.OData.Building;
using NuClear.EntityDataModel.EntityFramework.Building;
using NuClear.Metamodeling.Provider.Sources;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.QueryExecution.Tests
{
    public class QueryExecutionBaseFixture : EdmxBuilderBaseFixture
    {
        protected static IEdmModel BuildEdmModel(IMetadataSource source, ITypeProvider typeProvider = null)
        {
            var metadataProvider = CreateMetadataProvider(source);
            var context = LookupContext(metadataProvider);

            var dbModel = BuildModel(context, typeProvider);
            var clrTypes = dbModel.GetClrTypes();

            var edmModel = EdmModelBuilder.Build(context);
            edmModel.AddClrAnnotations(clrTypes);

            return edmModel;
        }
    }

    [TestFixture]
    public sealed class CustomerIntelligenceTests : QueryExecutionBaseFixture
    {
        [Test]
        public void Test1()
        {
            var dbModel = CreateCustomerIntelligenceModel();
            var edmModel = CreateCustomerIntelligenceEdmModel();
        }

        private static DbModel CreateCustomerIntelligenceModel()
        {
            return BuildModel(CustomerIntelligenceMetadataSource, CustomerIntelligenceTypeProvider);
        }

        private static IEdmModel CreateCustomerIntelligenceEdmModel()
        {
            return BuildEdmModel(CustomerIntelligenceMetadataSource, CustomerIntelligenceTypeProvider);
        }
    }
}