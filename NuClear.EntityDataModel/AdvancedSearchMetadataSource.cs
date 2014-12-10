using System;
using System.Collections.Generic;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Concrete.Hierarchy;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Provider.Sources;

namespace NuClear.EntityDataModel
{
    public sealed class AdvancedSearchMetadataSource : MetadataSourceBase<AdvancedSearchIdentity>
    {
        private readonly IReadOnlyDictionary<Uri, IMetadataElement> _metadata;

        private readonly BoundedContextElement _customerIntelligence = 
            BoundedContextElement.Config
                .Name("CustomerIntelligence")
                .Elements(
                    EntityElement.Config
                        .Name("Firm")
                        .Property(EntityPropertyElement.Config.Name("Id").OfType(TypeCode.Int64))
                        .Property(EntityPropertyElement.Config.Name("OrganizationUnitId").OfType(TypeCode.Int64))
                        .Property(EntityPropertyElement.Config.Name("TerritoryId").OfType(TypeCode.Int64))
                        .Property(EntityPropertyElement.Config.Name("CreatedOn").OfType(TypeCode.DateTime))
                        .Property(EntityPropertyElement.Config.Name("LastQualifiedOn").OfType(TypeCode.DateTime))
                        .Property(EntityPropertyElement.Config.Name("LastDistributedOn").OfType(TypeCode.DateTime))
                        .Property(EntityPropertyElement.Config.Name("HasWebsite").OfType(TypeCode.Boolean))
                        .Property(EntityPropertyElement.Config.Name("HasPhone").OfType(TypeCode.Boolean))
                        .Property(EntityPropertyElement.Config.Name("CategoryGroup").OfType(TypeCode.Byte))
                        .Property(EntityPropertyElement.Config.Name("AddressCount").OfType(TypeCode.UInt32))
                        .Relation(EntityRelationElement.Config
                            .Name("Categories")
                            .DirectTo(
                                EntityElement.Config
                                    .Property(EntityPropertyElement.Config.Name("Id").OfType(TypeCode.Int64))
                                    .Property(EntityPropertyElement.Config.Name("Name").OfType(TypeCode.String))
                                    .Property(EntityPropertyElement.Config.Name("CategoryGroup").OfType(TypeCode.Byte))
                            )
                            .AsMany())
                        .Relation(EntityRelationElement.Config
                            .Name("Client")
                            .DirectTo(
                                EntityElement.Config
                                    .Property(EntityPropertyElement.Config.Name("Id").OfType(TypeCode.Int64))
                                    .Property(EntityPropertyElement.Config.Name("CategoryGroup").OfType(TypeCode.Byte))
                                    .Relation(
                                        EntityRelationElement.Config
                                            .Name("Accounts")
                                            .DirectTo(
                                                EntityElement.Config
                                                    .Name("Account")
                                                    .Property(EntityPropertyElement.Config.Name("Balance").OfType(TypeCode.Decimal))
                                            )
                                            .AsManyOptionally()
                                    )
                                    .Relation(
                                        EntityRelationElement.Config
                                            .Name("Contacts")
                                            .DirectTo(
                                                EntityElement.Config
                                                    .Property(EntityPropertyElement.Config.Name("Id").OfType(TypeCode.Int64))
                                                    .Property(EntityPropertyElement.Config.Name("Role").OfType(TypeCode.String))
                                                    .Property(EntityPropertyElement.Config.Name("IsFired").OfType(TypeCode.Boolean))
                                            )
                                            .AsManyOptionally()
                                    )
                            )
                            .AsSingleOptionally())
                );

        public AdvancedSearchMetadataSource()
        {
            HierarchyMetadata root = HierarchyMetadata.Config
                .Id.Is(IdBuilder.For<AdvancedSearchIdentity>())
                .Childs(_customerIntelligence);
            _metadata = new Dictionary<Uri, IMetadataElement> { { root.Identity.Id, root } };
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata
        {
            get
            {
                return _metadata;
            }
        }
    }
}