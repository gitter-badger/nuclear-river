using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.OData;

using Microsoft.OData.Edm;

using NuClear.AdvancedSearch.EntityDataModel.EntityFramework.Emit;
using NuClear.AdvancedSearch.EntityDataModel.Metadata;
using NuClear.AdvancedSearch.EntityDataModel.OData.Building;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Processors;
using NuClear.Metamodeling.Provider;
using NuClear.Metamodeling.Provider.Sources;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.QueryExecution.Tests
{
    /// <remarks>
    /// Executes in invariant culture to simplify expected result after the formatting.
    /// </remarks>>
    [TestFixture, SetCulture("")]
    public sealed class CustomerIntelligenceTests : QueryExecutionBaseFixture
    {
        private readonly IDictionary<Uri, IEdmModel> Models = BuildModels(MetadataProvider, BusinessDirectoryId, CustomerIntelligenceId);

        [TestCase(BusinessDirectory, "Category", null, Result = "Category[]")]
        [TestCase(BusinessDirectory, "CategoryGroup", null, Result = "CategoryGroup[]")]
        [TestCase(BusinessDirectory, "OrganizationUnit", null, Result = "OrganizationUnit[]")]
        [TestCase(BusinessDirectory, "Territory", null, Result = "Territory[]")]
        public string ShouldBuildQuery(string modelName, string type, string filter)
        {
            return BuildQuery(modelName, type, filter);
        }

        /// <summary>
        /// Критерии взяты из https://confluence.2gis.ru/pages/viewpage.action?pageId=143462711.
        /// </summary>
        [TestCase("$filter=CreatedOn lt @date&@date=2015-01-01T00:00Z", Result = "Firm[].Where($it => ($it.CreatedOn < 01/01/2015 00:00:00 +00:00))", Description = "Поиск по дате создания.")]
        [TestCase("$filter=LastDisqualifiedOn lt @date&@date=2015-01-01T00:00Z", Result = "Firm[].Where($it => ($it.LastDisqualifiedOn < Convert(01/01/2015 00:00:00 +00:00)))", Description = "Поиск по дате последнего возвращения в резерв.")]
        [TestCase("$filter=LastDistributedOn lt @date&@date=2015-01-01T00:00Z", Result = "Firm[].Where($it => ($it.LastDistributedOn < Convert(01/01/2015 00:00:00 +00:00)))", Description = "Поиск по дате последнего размещения.")]
        [TestCase("$filter=HasPhone eq @value&@value=true", Result = "Firm[].Where($it => ($it.HasPhone == True))", Description = "Поиск по наличию телефона.")]
        [TestCase("$filter=HasWebsite eq @value&@value=true", Result = "Firm[].Where($it => ($it.HasWebsite == True))", Description = "Поиск по наличию сайта.")]
        [TestCase("$filter=AddressCount gt @amount&@amount=10", Result = "Firm[].Where($it => ($it.AddressCount > 10))", Description = "Поиск по количеству активных адресов.")]
        [TestCase("$filter=CategoryGroupId eq @id&@id=12345", Result = "Firm[].Where($it => ($it.CategoryGroupId == Convert(12345)))", Description = "Поиск по ценовой категории фирмы.")]
        [TestCase("$filter=OrganizationUnitId eq @id&@id=12345", Result = "Firm[].Where($it => ($it.OrganizationUnitId == Convert(12345)))", Description = "Поиск по организации.")]
        [TestCase("$filter=TerritoryId eq  @id&@id=12345", Result = "Firm[].Where($it => ($it.TerritoryId == Convert(12345)))", Description = "Поиск по территории.")]
        [TestCase("$filter=Categories/any(x:x/CategoryId eq @id1)&@id1=123", Result = "Firm[].Where($it => $it.Categories.Any(x => (x.CategoryId == Convert(123))))", Description = "Поиск по рубрике 1-го уровня.")]
        [TestCase("$filter=Categories/any(x:x/CategoryId eq @id2)&@id2=123", Result = "Firm[].Where($it => $it.Categories.Any(x => (x.CategoryId == Convert(123))))", Description = "Поиск по рубрике 2-го уровня.")]
        [TestCase("$filter=Categories/any(x:x/CategoryId eq @id3)&@id3=123", Result = "Firm[].Where($it => $it.Categories.Any(x => (x.CategoryId == Convert(123))))", Description = "Поиск по рубрике 3-го уровня.")]
        [TestCase("$filter=CategoryGroups/any(x:x/CategoryGroupId eq @id)&@id=12345", Result = "Firm[].Where($it => $it.CategoryGroups.Any(x => (x.CategoryGroupId == Convert(12345))))", Description = "Поиск по ценовой категории рубрики.")]
        [TestCase("$filter=Client/CategoryGroupId eq @id&@id=12345", Result = "Firm[].Where($it => ($it.Client.CategoryGroupId == Convert(12345)))", Description = "Поиск по ценовой категории клиента.")]
        [TestCase("$filter=Client/Contacts/any(x:x/Role eq AdvancedSearch.CustomerIntelligence.ContactRole'Employee')", Result = "Firm[].Where($it => $it.Client.Contacts.Any(x => (Convert(x.Role) == Convert(Employee))))", Description = "Поиск по роли контакта.")]
        [TestCase("$filter=Client/Contacts/any(x:x/IsFired ne true)", Result = "Firm[].Where($it => $it.Client.Contacts.Any(x => (x.IsFired != True)))", Description = "Поиск по наличию рабочего контакта.")]
        [TestCase("$filter=Accounts/all(x:x/Balance gt @balance)&@balance=1000", Result = "Firm[].Where($it => $it.Accounts.All(x => (x.Balance > Convert(1000))))", Description = "Поиск по балансу лицевого счета.")]
        public string ShouldAcceptMainCriteria(string filter)
        {
            return BuildQuery(CustomerIntelligence, "Firm", filter);
        }

        private string BuildQuery(string modelName, string type, string filter)
        {
            var model = Models[Metadata.Id.For<AdvancedSearchIdentity>(modelName)];

            var firmType = LookupClrType(model, type);

            var options = CreateValidQueryOptions(model, firmType, filter);

            var query = options.ApplyTo(CreateDataSource(firmType), DefaultQuerySettings);

            var expression = ToExpression(query, firmType.Namespace);

            Debug.WriteLine(expression);

            return expression;
        }

        private static IReadOnlyDictionary<IMetadataElementIdentity,Type> EmitClrTypes(BoundedContextElement context)
        {
            var typeProvider = new EmitTypeProvider();
            foreach (var element in context.ConceptualModel.Entities)
            {
                typeProvider.Resolve(element);
            }
            return typeProvider.RegisteredTypes;
        }

        private static Type LookupClrType(IEdmModel model, string name)
        {
            var @namespace = model.DeclaredNamespaces.SingleOrDefault();
            var fullName = (string.IsNullOrEmpty(@namespace) ? "" : @namespace + ".") + name;

            var edmType = model.FindDeclaredType(fullName);
            if (edmType == null)
            {
                throw new Exception("The type was not found for the specified name.");
            }

            var annotation = model.GetAnnotationValue<ClrTypeAnnotation>(edmType);
            if (annotation == null)
            {
                throw new Exception("The CLR type cannot be resolved.");
            }

            return annotation.ClrType;
        }

        private static IDictionary<Uri, IEdmModel> BuildModels(IMetadataProvider provider, params Uri[] identities)
        {
            var models = new Dictionary<Uri, IEdmModel>();
            var builder = new EdmModelBuilder(provider);

            foreach (var identity in identities)
            {
                BoundedContextElement context;
                provider.TryGetMetadata(identity, out context);

                if (context != null)
                {
                    var clrTypes = EmitClrTypes(context);
                    var model = builder.Build(identity).AnnotateByClrTypes(elementId => clrTypes[elementId.AsIdentity()]);

                    models.Add(identity, model);
                }
            }

            return models;
        }

        #region Customer Intelligence

        private const string BusinessDirectory = "BusinessDirectory";
        private const string CustomerIntelligence = "CustomerIntelligence";

        private static readonly Uri BusinessDirectoryId = Metadata.Id.For<AdvancedSearchIdentity>(BusinessDirectory);
        private static readonly Uri CustomerIntelligenceId = Metadata.Id.For<AdvancedSearchIdentity>(CustomerIntelligence);

        private static IMetadataSource AdvancedSearchMetadataSource
        {
            get
            {
                return new AdvancedSearchMetadataSource();
            }
        }

        private static IMetadataProvider MetadataProvider
        {
            get
            {
                return new MetadataProvider(new[] { AdvancedSearchMetadataSource }, new IMetadataProcessor[0]);
            }
        }

        #endregion
    }
}