using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Xml;

using NUnit.Framework;

namespace NuClear.EntityDataModel.EntityFramework
{
    public class Firm
    {
        public long Id { get; set; }
        public long OrganizationUnitId { get; set; }
        public long TerritoryId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastQualifiedOn { get; set; }
        public DateTime LastDistributedOn { get; set; }
        public bool HasWebsite { get; set; }
        public bool HasPhone { get; set; }
        public byte CategoryGroup { get; set; }
        public long AddressCount { get; set; }
        
        public ICollection<Category> Categories { get; set; }
    }

    public class Category
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public byte CategoryGroup { get; set; }
    }

    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }

        public Address Address { get; set; }
        public virtual ICollection<Item> BoughtItems { get; set; }
    }

    public class Address
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
    }

    public class Item
    {
        public int ItemId { get; set; }
        public string Name { get; set; }
        public double InitialPrice { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? BuyerId { get; set; }
        public int? SuccessfulBidId { get; set; }

//        public virtual User Buyer { get; set; }
//        public virtual Bid SuccessfulBid { get; set; }
//        public virtual ICollection<Bid> Bids { get; set; }
//        public virtual ICollection<Category> Categories { get; set; }
    }

    internal class DemoFixture
    {
        private static readonly DbProviderInfo DefaultProviderInfo = new DbProviderInfo("System.Data.SqlClient", "2012");
        private static readonly IDbConnectionFactory DefaultFactory = new LocalDbConnectionFactory("mssqllocaldb");

        [Test]
        public void CreateDemoModel()
        {
            var builder = new DbModelBuilder();
            //builder.ComplexType<Address>();
            //builder.Entity<Address>();
            //builder.Entity<Address>();
            builder.Entity<Firm>();

            var model = builder.Build(DefaultProviderInfo);
            model.Dump();

            //var compiledModel = model.Compile();

//            var connection = DefaultFactory.CreateConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=D:\data.mdf");
//            using (var context = compiledModel.CreateObjectContext<ObjectContext>(connection))
//            {
//                //context.MetadataWorkspace.LoadFromAssembly(CreateAssembly());
//
//                //var records = new ObjectQuery<DbDataRecord>("SELECT t.Name FROM CodeFirstContainer.Tables as t", context);
//                var records = new ObjectQuery<DbDataRecord>("SELECT t.Name FROM Tables as t", context);
//                foreach (var record in records)
//                {
//                    Debug.WriteLine(record.GetString(record.GetOrdinal("Name")));
//                }
//            }
        }

        [Test]
        public void CreateModel()
        {
            var builder = new DbModelBuilder();

            var assembly = CreateAssembly();
            //builder.Configurations.AddFromAssembly(assembly);
            builder.RegisterEntityType(CreateType());

            var model = builder.Build(DefaultProviderInfo);
            model.Dump();

            var compiledModel = model.Compile();

//            var connection = DefaultFactory.CreateConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=D:\data.mdf");
//            using (var context = compiledModel.CreateObjectContext<ObjectContext>(connection))
//            {
//                //context.MetadataWorkspace.LoadFromAssembly(CreateAssembly());
//
//                //var records = new ObjectQuery<DbDataRecord>("SELECT t.Name FROM CodeFirstContainer.Tables as t", context);
//                var records = new ObjectQuery<DbDataRecord>("SELECT t.Name FROM Tables as t", context);
//                foreach (var record in records)
//                {
//                    Debug.WriteLine(record.GetString(record.GetOrdinal("Name")));
//                }
//            }
        }

        [Test, Explicit]
        public void BuildModel()
        {
            var builder = new DbModelBuilder();

            var model = builder.Build(DefaultProviderInfo);

            var idProperty = EdmProperty.CreatePrimitive("Id", PrimitiveType.GetEdmPrimitiveType(PrimitiveTypeKind.Int32));
            idProperty.Nullable = false;
            
            var nameProperty = EdmProperty.CreatePrimitive("Name", PrimitiveType.GetEdmPrimitiveType(PrimitiveTypeKind.String));
            var descProperty = EdmProperty.CreatePrimitive("Description", PrimitiveType.GetEdmPrimitiveType(PrimitiveTypeKind.String));
            var createdOnProperty = EdmProperty.CreatePrimitive("CreatedOn", PrimitiveType.GetEdmPrimitiveType(PrimitiveTypeKind.DateTime));

            var entityType = EntityType.Create("Table", "DataModel", DataSpace.CSpace, new[] { "Id" }, new[]
                                                                                                          {
                                                                                                              idProperty, 
                                                                                                              nameProperty,
                                                                                                              descProperty,
                                                                                                              createdOnProperty
                                                                                                          }, 
                                                                                                          new MetadataProperty[]
                                                                                                          {
                                                                                                              MetadataProperty.CreateAnnotation("http://schemas.microsoft.com/ado/2013/11/edm/customannotation:ClrType", CreateType())
                                                                                                          });
            model.ConceptualModel.AddItem(entityType);

            var entitySet = EntitySet.Create("Tables", "schema", "Table", null, entityType, new MetadataProperty[0]);

            model.ConceptualModel.Container.AddEntitySetBase(entitySet);

            //model.ConceptualModel.Dump();




            var idProperty2 = EdmProperty.Create("Id", model.ProviderManifest.GetStoreType(idProperty.TypeUsage));
            idProperty2.Nullable = false;

            var nameProperty2 = EdmProperty.Create("Name", model.ProviderManifest.GetStoreType(nameProperty.TypeUsage));
            var descProperty2 = EdmProperty.Create("Description", model.ProviderManifest.GetStoreType(descProperty.TypeUsage));
            var createdOnProperty2 = EdmProperty.Create("CreatedOn", model.ProviderManifest.GetStoreType(createdOnProperty.TypeUsage));

            var entityType2 = EntityType.Create("Table", "DataModel.Store", DataSpace.SSpace, new[] { "Id" }, new[]
                                                                                                          {
                                                                                                              idProperty2, 
                                                                                                              nameProperty2,
                                                                                                              descProperty2,
                                                                                                              createdOnProperty2
                                                                                                          }, new MetadataProperty[0]);

            model.StoreModel.AddItem(entityType2);

            var storeSet = EntitySet.Create("Tables", "dbo", "Table", null, entityType2, new MetadataProperty[0]);
            model.StoreModel.Container.AddEntitySetBase(storeSet);


//            model.StoreModel.Dump();

            var mapping = new EntitySetMapping(entitySet, model.ConceptualToStoreMapping);
            
            var entityTypeMapping = new EntityTypeMapping(mapping);
            entityTypeMapping.AddType(entityType);

            var mappingFragment = new MappingFragment(storeSet, entityTypeMapping, false);
            mappingFragment.AddPropertyMapping(new ScalarPropertyMapping(idProperty, idProperty2));
            mappingFragment.AddPropertyMapping(new ScalarPropertyMapping(nameProperty, nameProperty2));
            mappingFragment.AddPropertyMapping(new ScalarPropertyMapping(descProperty, descProperty2));
            mappingFragment.AddPropertyMapping(new ScalarPropertyMapping(createdOnProperty, createdOnProperty2));

            entityTypeMapping.AddFragment(mappingFragment);

            mapping.AddTypeMapping(entityTypeMapping);

            model.ConceptualToStoreMapping.AddSetMapping(mapping);


            //model.Dump();


            var compiledModel = model.Compile();

            var connection = DefaultFactory.CreateConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=D:\data.mdf");
            using (var context = compiledModel.CreateObjectContext<ObjectContext>(connection))
            {
                //context.MetadataWorkspace.LoadFromAssembly(CreateAssembly());

                //var records = new ObjectQuery<DbDataRecord>("SELECT t.Name FROM CodeFirstContainer.Tables as t", context);
                var records = new ObjectQuery<DbDataRecord>("SELECT t.Name FROM Tables as t", context);
                foreach (var record in records)
                {
                    Debug.WriteLine(record.GetString(record.GetOrdinal("Name")));
                }
            }
        }


        [Test, Explicit]
        public void Test()
        {
//            var factory = new System.Data.Entity.Infrastructure.LocalDbConnectionFactory("mssqllocaldb");
//            var dbConnection = factory.CreateConnection("DataEntities");
//            var metadata = new MetadataWorkspace(CSpace, SSpace, CSSpace, OSpace);
//            var connection = new EntityConnection(metadata, dbConnection);

//            var connection = new EntityConnection("name=DataEntities");
//
//            using (var context = new ObjectContext(connection))
//            {
//                context.MetadataWorkspace.LoadFromAssembly(CreateAssembly());
//
//                var records = new ObjectQuery<DbDataRecord>("SELECT t.Name FROM DataEntities.CTables as t", context);
//                foreach (var record in records)
//                {
//                    Debug.WriteLine(record.GetString(record.GetOrdinal("Name")));
//                }
//            }
        }

        private static Assembly CreateAssembly()
        {
            var assemblyBuilder = EmitHelper.DefineAssembly("ObjectSpace");
            var moduleBuilder = assemblyBuilder.DefineModule("CustomerIntelligence");

            var tableTypeBuilder = moduleBuilder.DefineType("DataEntities.Table");

            // add public fields to match the source object
            foreach (var sourceField in new[]
                                              {
                                                  new {Name = "Id", FieldType = typeof(Int32)},
                                                  new {Name = "Name", FieldType = typeof(string)},
                                                  new {Name = "Description", FieldType = typeof(string)},
                                                  new {Name = "CreatedOn", FieldType = typeof(DateTime?)},
                                              })
            {
                tableTypeBuilder.DefineProperty(sourceField.Name, sourceField.FieldType);
            }

            // create the dynamic class
            tableTypeBuilder.CreateType();

            return assemblyBuilder;
        }

        private static Type CreateType()
        {
            var assemblyBuilder = EmitHelper.DefineAssembly("ObjectSpace");
            var moduleBuilder = assemblyBuilder.DefineModule("CustomerIntelligence");

            var tableTypeBuilder = moduleBuilder.DefineType("DataEntities.Table");

            // add public fields to match the source object
            foreach (var sourceField in new[]
                                              {
                                                  new {Name = "Id", FieldType = typeof(Int32)},
                                                  new {Name = "Name", FieldType = typeof(string)},
                                                  new {Name = "Description", FieldType = typeof(string)},
                                                  new {Name = "CreatedOn", FieldType = typeof(DateTime?)},
                                              })
            {
                tableTypeBuilder.DefineProperty(sourceField.Name, sourceField.FieldType);
            }

            // create the dynamic class
            return tableTypeBuilder.CreateType();
        }
  
    }

    internal static class EmitHelper
    {
        // the property set and property get methods require a special set of attributes
        private const MethodAttributes PropertyAccessorAttributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

        public static AssemblyBuilder DefineAssembly(string assemblyName)
        {
            return AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Run);
        }

        public static ModuleBuilder DefineModule(this AssemblyBuilder assemblyBuilder, string moduleName)
        {
            return assemblyBuilder.DefineDynamicModule(moduleName);
        }

        public static TypeBuilder DefineType(this ModuleBuilder moduleBuilder, string name)
        {
            return moduleBuilder.DefineType(name, TypeAttributes.Public);
        }

        public static void DefineProperty(this TypeBuilder typeBuilder, string propertyName, Type propertyType)
        {
            var fieldBuilder = typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

            var propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);

            // define the "get" accessor method
            var getMethodBuilder = typeBuilder.DefineMethod("get_" + propertyName, PropertyAccessorAttributes, propertyType, Type.EmptyTypes);
            var getMethodIL = getMethodBuilder.GetILGenerator();
            getMethodIL.Emit(OpCodes.Ldarg_0);
            getMethodIL.Emit(OpCodes.Ldfld, fieldBuilder);
            getMethodIL.Emit(OpCodes.Ret);

            // define the "set" accessor method
            var setMethodBuilder = typeBuilder.DefineMethod("set_" + propertyName, PropertyAccessorAttributes, null, new[] { propertyType });
            var setMethodIL = setMethodBuilder.GetILGenerator();
            setMethodIL.Emit(OpCodes.Ldarg_0);
            setMethodIL.Emit(OpCodes.Ldarg_1);
            setMethodIL.Emit(OpCodes.Stfld, fieldBuilder);
            setMethodIL.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getMethodBuilder);
            propertyBuilder.SetSetMethod(setMethodBuilder);
        }

    }

    internal static class EdmModelExtensions
    {
        public static DbProviderInfo GetProviderInfo(this DbConnection connection/*, out DbProviderManifest providerManifest*/)
        {
            var resolver = DbConfiguration.DependencyResolver.GetService<IManifestTokenResolver>();
            var str = resolver.ResolveManifestToken(connection);

            DbProviderInfo dbProviderInfo = new DbProviderInfo(GetProviderInvariantName(connection), str);
            ///providerManifest = DbProviderServices.GetProviderServices(connection).GetProviderManifest(str);
            return dbProviderInfo;
        }

        public static string GetProviderInvariantName(this DbConnection connection)
        {
            return DbConfiguration.DependencyResolver.GetService<IProviderInvariantName>(DbProviderServices.GetProviderFactory(connection)).Name;
        }




        public static void Dump(this DbModel model)
        {
            var stringBuilder = new StringBuilder();
            using (var xmlWriter = XmlWriter.Create(stringBuilder, new XmlWriterSettings { Indent = true }))
            {
                EdmxWriter.WriteEdmx(model, xmlWriter);
                //edmModel.ValidateAndSerializeCsdl(xmlWriter);
            }

            Debug.WriteLine(stringBuilder.ToString());
        }

        public static void Dump(this EdmModel edmModel)
        {
            var stringBuilder = new StringBuilder();
            using (var xmlWriter = XmlWriter.Create(stringBuilder, new XmlWriterSettings { Indent = true }))
            {
                edmModel.ValidateAndSerializeCsdl(xmlWriter);
            }

            Debug.WriteLine(stringBuilder.ToString());
        }

        public static void ValidateAndSerializeCsdl(this EdmModel model, XmlWriter writer)
        {
            var csdlErrors = SerializeAndGetCsdlErrors(model, writer);


            if (csdlErrors.Count > 0)
            {
                Debug.WriteLine(ToErrorMessage(csdlErrors));
            }
        }

        private static List<DataModelErrorEventArgs> SerializeAndGetCsdlErrors(this EdmModel model, XmlWriter writer)
        {
            List<DataModelErrorEventArgs> validationErrors = new List<DataModelErrorEventArgs>();
            CsdlSerializer csdlSerializer = new CsdlSerializer();
            csdlSerializer.OnError += (EventHandler<DataModelErrorEventArgs>)((s, e) => validationErrors.Add(e));
            csdlSerializer.Serialize(model, writer, (string)null);
            return validationErrors;
        }

        public static string ToErrorMessage(this IEnumerable<DataModelErrorEventArgs> validationErrors)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Validation Errors:");
            stringBuilder.AppendLine();
            foreach (DataModelErrorEventArgs modelErrorEventArgs in validationErrors)
                stringBuilder.AppendFormat("{0}.{1}: {2}", modelErrorEventArgs.Item, modelErrorEventArgs.PropertyName, modelErrorEventArgs.ErrorMessage);
            return stringBuilder.ToString();
        }
    }
}
