using NuClear.AdvancedSearch.ServiceBus.Contracts.DTO;

using ProtoBuf.Meta;

namespace NuClear.AdvancedSearch.ServiceBus
{
    public sealed class AdvancedSearchTypeModel
    {
        public static TypeModel CreateTypeModel()
        {
            var typeModel = TypeModel.Create();

            typeModel.Add(typeof(TrackedUseCase), false)
                    .Add(2, "Operations");
            typeModel.Add(typeof(OperationScopeNode), false)
                    .Add(4, "ChangesContext");
            typeModel.Add(typeof(EntityChangesContext), false)
                    .Add(1, "Store");
            typeModel.Add(typeof(ChangesDescriptor), false)
                    .Add(1, "Id")
                    .Add(2, "Details");
            typeModel.Add(typeof(ChangesDetail), false)
                    .Add(1, "ChangesType");

            var compiledTypeModel = typeModel.Compile();
            return compiledTypeModel;
        }
    }
}
