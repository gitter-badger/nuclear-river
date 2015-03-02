using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.OData;

using Microsoft.OData.Edm;

using NuClear.AdvancedSearch.EntityDataModel.EntityFramework.Emit;
using NuClear.AdvancedSearch.EntityDataModel.Metadata;
using NuClear.AdvancedSearch.EntityDataModel.OData.Building;
using NuClear.Metamodeling.Elements.Identities;
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
        public IEdmModel EdmModel { get; private set; }
            
        [TestFixtureSetUp]
        public void Setup()
        {
            var metadataProvider = MetadataProvider;

            BoundedContextElement context;
            metadataProvider.TryGetMetadata(CustomerIntelligenceId, out context);

            var clrTypes = EmitClrTypes(context);
            var modelBuilder = new EdmModelBuilder(metadataProvider);
            EdmModel = modelBuilder.Build(CustomerIntelligenceId).AnnotateByClrTypes(elementId => clrTypes[elementId.AsIdentity()]);
        }

        /// <summary>
        /// Критерии взяты из https://confluence.2gis.ru/pages/viewpage.action?pageId=143462711.
        /// </summary>
        [TestCase("$filter=OrganizationUnit/Id eq @id&@id=12345", Result = "Firm[].Where($it => ($it.OrganizationUnit.Id == Convert(12345)))", Description = "Поиск по организации.")]
        [TestCase("$filter=Territory/Id eq  @id&@id=12345", Result = "Firm[].Where($it => ($it.Territory.Id == Convert(12345)))", Description = "Поиск по территории.")]
        [TestCase("$filter=CreatedOn lt @date&@date=2015-01-01T00:00Z", Result = "Firm[].Where($it => ($it.CreatedOn < 01/01/2015 00:00:00 +00:00))", Description = "Поиск по дате создания.")]
        [TestCase("$filter=LastDisqualifiedOn lt @date&@date=2015-01-01T00:00Z", Result = "Firm[].Where($it => ($it.LastDisqualifiedOn < Convert(01/01/2015 00:00:00 +00:00)))", Description = "Поиск по дате последнего возвращения в резерв.")]
        [TestCase("$filter=LastDistributedOn lt @date&@date=2015-01-01T00:00Z", Result = "Firm[].Where($it => ($it.LastDistributedOn < Convert(01/01/2015 00:00:00 +00:00)))", Description = "Поиск по дате последнего размещения.")]
        [TestCase("$filter=Categories1/any(x:x/Category/Id eq @id1)&@id1=123", Result = "Firm[].Where($it => $it.Categories1.Any(x => (x.Category.Id == Convert(123))))", Description = "Поиск по рубрике 1-го уровня.")]
        [TestCase("$filter=Categories2/any(x:x/Category/Id eq @id2)&@id2=123", Result = "Firm[].Where($it => $it.Categories2.Any(x => (x.Category.Id == Convert(123))))", Description = "Поиск по рубрике 2-го уровня.")]
        [TestCase("$filter=Categories3/any(x:x/Category/Id eq @id3)&@id3=123", Result = "Firm[].Where($it => $it.Categories3.Any(x => (x.Category.Id == Convert(123))))", Description = "Поиск по рубрике 3-го уровня.")]
        [TestCase("$filter=CategoryGroup eq AdvancedSearch.CustomerIntelligence.CategoryGroup'Percent120'", Result = "Firm[].Where($it => (Convert($it.CategoryGroup) == Convert(Percent120)))", Description = "Поиск по ценовой категории фирмы.")]
        [TestCase("$filter=Categories3/any(x:x/CategoryGroup eq AdvancedSearch.CustomerIntelligence.CategoryGroup'Percent120')", Result = "Firm[].Where($it => $it.Categories3.Any(x => (Convert(x.CategoryGroup) == Convert(Percent120))))", Description = "Поиск по ценовой категории рубрики.")]
        [TestCase("$filter=Client/CategoryGroup eq AdvancedSearch.CustomerIntelligence.CategoryGroup'Percent120'", Result = "Firm[].Where($it => (Convert($it.Client.CategoryGroup) == Convert(Percent120)))", Description = "Поиск по ценовой категории клиента.")]
        [TestCase("$filter=AddressCount gt @amount&@amount=10", Result = "Firm[].Where($it => ($it.AddressCount > 10))", Description = "Поиск по количеству активных адресов.")]
        [TestCase("$filter=HasWebsite eq @value&@value=true", Result = "Firm[].Where($it => ($it.HasWebsite == True))", Description = "Поиск по наличию сайта.")]
        [TestCase("$filter=HasPhone eq @value&@value=true", Result = "Firm[].Where($it => ($it.HasPhone == True))", Description = "Поиск по наличию телефона.")]
        [TestCase("$filter=Client/Contacts/any(x:x/Role eq AdvancedSearch.CustomerIntelligence.ContactRole'Employee')", Result = "Firm[].Where($it => $it.Client.Contacts.Any(x => (Convert(x.Role) == Convert(Employee))))", Description = "Поиск по роли контакта.")]
        [TestCase("$filter=Client/Contacts/any(x:x/IsFired ne true)", Result = "Firm[].Where($it => $it.Client.Contacts.Any(x => (x.IsFired != True)))", Description = "Поиск по наличию рабочего контакта.")]
        [TestCase("$filter=Client/Accounts/all(x:x/Balance gt @balance)&@balance=1000", Result = "Firm[].Where($it => $it.Client.Accounts.All(x => (x.Balance > Convert(1000))))", Description = "Поиск по балансу лицевого счета.")]
        public string ShouldAcceptMainCriteria(string filter)
        {
            var firmType = LookupClrType("Firm");

            var options = CreateValidQueryOptions(EdmModel, firmType, filter);

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

        private Type LookupClrType(string name, string @namespace = "AdvancedSearch.CustomerIntelligence")
        {
            var fullName = (string.IsNullOrEmpty(@namespace) ? "" : @namespace + ".") + name;
            
            var edmType = EdmModel.FindDeclaredType(fullName);
            if (edmType == null)
            {
                throw new Exception("The type was not found for the specified name.");
            }

            var annotation = EdmModel.GetAnnotationValue<ClrTypeAnnotation>(edmType);
            if (annotation == null)
            {
                throw new Exception("The CLR type cannot be resolved.");
            }

            return annotation.ClrType;
        }

        #region Customer Intelligence

        private static readonly Uri CustomerIntelligenceId = IdBuilder.For<AdvancedSearchIdentity>("CustomerIntelligence");

        private static IMetadataSource CustomerIntelligenceMetadataSource
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
                return new MetadataProvider(new[] { CustomerIntelligenceMetadataSource }, new IMetadataProcessor[0]);
            }
        }

        #endregion
    }
}