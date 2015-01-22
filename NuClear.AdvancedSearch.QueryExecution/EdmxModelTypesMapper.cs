using System.Data.Entity.Infrastructure;

using Microsoft.OData.Edm;

namespace NuClear.AdvancedSearch.QueryExecution
{
    public interface IEdmModelTypesMapper
    {
        IEdmModel MapTypes(IEdmModel model);
    }

    public sealed class EdmxModelTypesMapper : IEdmModelTypesMapper
    {
        private readonly DbModel _dbModel;

        public EdmxModelTypesMapper(DbModel dbModel)
        {
            _dbModel = dbModel;
        }

        public IEdmModel MapTypes(IEdmModel model)
        {
            var types = _dbModel.GetClrTypes();
            model.AddClrAnnotations(types);
            return model;
        }
    }
}