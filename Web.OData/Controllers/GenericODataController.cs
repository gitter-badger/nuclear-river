using System.Linq;
using System.Web.OData;

namespace NuClear.AdvancedSearch.Web.OData.Controllers
{
    public class GenericODataController<T> : ODataController
    {
        private readonly StoreHelper _storeHelper;

        public GenericODataController(StoreHelper storeHelper)
        {
            _storeHelper = storeHelper;
        }

        [EnableQuery]
        public IQueryable<T> Get()
        {
            var entities = _storeHelper.GetEntities<T>();

            return entities;
        }
    }
}