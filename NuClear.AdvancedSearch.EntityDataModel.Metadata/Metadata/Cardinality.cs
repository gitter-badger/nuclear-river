namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    public enum Cardinality : byte
    {
        /// <summary>
        /// Specifies the relation that must direct to an entity.
        /// </summary>
        One,
        /// <summary>
        /// Specifies the relation that may direct to an entity.
        /// </summary>
        OptionalOne,
        /// <summary>
        /// Specifies the relation that must direct to at least an entity.
        /// </summary>
        Many,
        /// <summary>
        /// Specifies the relation that may direct to entities.
        /// </summary>
        OptionalMany,
    }
}