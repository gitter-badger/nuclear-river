using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.OData.Query;

using Microsoft.OData.Edm;

using NuClear.AdvancedSearch.EntityDataModel.EntityFramework.Tests.Model.CustomerIntelligence;
using NuClear.AdvancedSearch.EntityDataModel.Metadata;
using NuClear.AdvancedSearch.EntityDataModel.OData.Building;
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

            var modelBuilder = new EdmModelBuilder(metadataProvider);
            var contextId = metadataProvider.Metadata.Metadata.Values.OfType<BoundedContextElement>().Single().Identity.Id;

            EdmModel = modelBuilder.Build(contextId).AnnotateByClrTypes(CustomerIntelligenceClrTypes);
        }

        /// <summary>
        /// Критерии взяты из https://confluence.2gis.ru/pages/viewpage.action?pageId=143462711.
        /// </summary>
        [TestCase("$filter=OrganizationUnit/Id eq 12345", Result = "Firm[].Where($it => ($it.OrganizationUnit.Id == 12345))", Description = "Поиск по организации.")]
        [TestCase("$filter=Territory/Id eq 12345", Result = "Firm[].Where($it => ($it.Territory.Id == 12345))", Description = "Поиск по территории.")]
        [TestCase("$filter=CreatedOn lt 2015-01-01T00:00Z", Result = "Firm[].Where($it => ($it.CreatedOn < 01/01/2015 00:00:00 +00:00))", Description = "Поиск по дате создания.")]
        [TestCase("$filter=LastDisqualifiedOn lt 2015-01-01T00:00Z", Result = "Firm[].Where($it => ($it.LastDisqualifiedOn < Convert(01/01/2015 00:00:00 +00:00)))", Description = "Поиск по дате последнего возвращения в резерв.")]
        [TestCase("$filter=LastDistributedOn lt 2015-01-01T00:00Z", Result = "Firm[].Where($it => ($it.LastDistributedOn < Convert(01/01/2015 00:00:00 +00:00)))", Description = "Поиск по дате последнего размещения.")]
        [TestCase("$filter=Categories/any(x:x/Category/Id eq 123)", Result = "Firm[].Where($it => $it.Categories.Any(x => (x.Category.Id == 123)))", Description = "Поиск по рубрике 1-го уровня.")]
        [TestCase("$filter=Categories/any(x:x/Category/Id eq 123)", Result = "Firm[].Where($it => $it.Categories.Any(x => (x.Category.Id == 123)))", Description = "Поиск по рубрике 2-го уровня.")]
        [TestCase("$filter=Categories/any(x:x/Category/Id eq 123)", Result = "Firm[].Where($it => $it.Categories.Any(x => (x.Category.Id == 123)))", Description = "Поиск по рубрике 3-го уровня.")]
        [TestCase("$filter=CategoryGroup/Id eq 123", Result = "Firm[].Where($it => ($it.CreatedOn < 01/01/2015 00:00:00 +00:00))", Description = "Поиск по ценовой категории фирмы.")]
        [TestCase("$filter=Categories/any(x:x/CategoryGroup/Id eq @categoryGroupId)", Result = "Firm[].Where($it => ($it.CreatedOn < 01/01/2015 00:00:00 +00:00))", Description = "Поиск по ценовой категории рубрики.")]
        [TestCase("$filter=Client/CategoryGroup/Id eq @categoryGroupId", Result = "Firm[].Where($it => ($it.CreatedOn < 01/01/2015 00:00:00 +00:00))", Description = "Поиск по ценовой категории клиента.")]
        [TestCase("$filter=AddressCount gt 10", Result = "Firm[].Where($it => ($it.AddressCount > 10))", Description = "Поиск по количеству активных адресов.")]
        [TestCase("$filter=HasWebsite eq true", Result = "Firm[].Where($it => ($it.HasWebsite == True))", Description = "Поиск по наличию сайта.")]
        [TestCase("$filter=HasPhone eq true", Result = "Firm[].Where($it => ($it.HasPhone == True))", Description = "Поиск по наличию телефона.")]
        [TestCase("$filter=Client/Contacts/any(x:x/Role eq AdvancedSearch.CustomerIntelligence.ContactRole'Employee')", Result = "Firm[].Where($it => ($it.CreatedOn < 01/01/2015 00:00:00 +00:00))", Description = "Поиск по роли контакта.")]
        [TestCase("$filter=Client/Contacts/any(x:x/IsFired ne true)", Result = "Firm[].Where($it => $it.Client.Contacts.Any(x => (x.IsFired != True)))", Description = "Поиск по наличию рабочего контакта.")]
        [TestCase("$filter=Client/Accounts/all(x:x/Balance gt 1000)", Result = "Firm[].Where($it => $it.Client.Accounts.All(x => (x.Balance > 1000)))", Description = "Поиск по балансу лицевого счета.")]
        public string ShouldAcceptMainCriteria(string filter)
        {
            var options = CreateValidQueryOptions<Firm>(EdmModel, filter);

            var query = options.ApplyTo(CustomerIntelligenceDataSource, DefaultQuerySettings);

            var expression = ToExpression<Firm>(query);

            Debug.WriteLine(expression);

            return expression;
        }

        #region Customer Intelligence

        private static IMetadataSource CustomerIntelligenceMetadataSource
        {
            get
            {
                return new AdvancedSearchMetadataSource();
            }
        }

        private static IQueryable CustomerIntelligenceDataSource
        {
            get
            {
                return CreateDataSource<Firm>();
            }
        }

        private static IMetadataProvider MetadataProvider
        {
            get
            {
                return new MetadataProvider(new[] { CustomerIntelligenceMetadataSource }, new IMetadataProcessor[0]);
            }
        }

        private static IReadOnlyDictionary<string, Type> CustomerIntelligenceClrTypes
        {
            get
            {
                return new[]
                       {
                           typeof(OrganizationUnit), 
                           typeof(Territory), 
                           typeof(Account), 
                           typeof(Category), 
                           typeof(Client), 
                           typeof(Contact), 
                           typeof(Firm), 
                           typeof(FirmCategory)
                       }
                        .ToDictionary(type => type.Name);
            }
        }

        #endregion
    }
}