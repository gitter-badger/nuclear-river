using System.Diagnostics;
using System.Linq;
using System.Web.OData.Query;

using Microsoft.OData.Core;
using Microsoft.OData.Edm;

namespace NuClear.AdvancedSearch.QueryExecution.Tests
{
    public abstract class QueryExecutionBaseFixture
    {
        protected static readonly ODataQuerySettings DefaultQuerySettings = new ODataQuerySettings
                                                                          {
                                                                              EnableConstantParameterization = false,
                                                                              HandleNullPropagation = HandleNullPropagationOption.False
                                                                          };

        protected readonly static ODataValidationSettings DefaultValidationSettings = new ODataValidationSettings();

        protected static ODataQueryOptions CreateValidQueryOptions<T>(IEdmModel model, string filter = null)
        {
            var options = CreateQueryOptions<T>(model, filter);
            try
            {
                options.Validate(DefaultValidationSettings);
            }
            catch (ODataException e)
            {
                Debug.WriteLine(e);
                throw;
            }
            return options;
        }

        protected static ODataQueryOptions CreateQueryOptions<T>(IEdmModel model, string query = null)
        {
            var request = TestHelper.CreateRequest(query);
            var queryOptions = TestHelper.CreateQueryOptions(model, typeof(T), request);
            return queryOptions;
        }

        protected static IQueryable<T> CreateDataSource<T>()
        {
            return Enumerable.Empty<T>().AsQueryable();
        }

        protected static string ToExpression<T>(IQueryable queryable)
        {
            var expression = queryable.Expression.ToString();
            
            // just reduce the text size
            expression = expression.Replace(typeof(T).Namespace + ".", "").Replace("\"", "'");

            return expression;
        }
    }
}