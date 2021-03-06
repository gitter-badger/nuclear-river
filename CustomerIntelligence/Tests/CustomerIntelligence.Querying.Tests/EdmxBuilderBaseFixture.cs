﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Reflection;

using Effort;
using Effort.DataLoaders;
using Effort.Provider;

using Moq;

using NuClear.AdvancedSearch.Common.Metadata.Elements;
using NuClear.AdvancedSearch.Common.Metadata.Identities;
using NuClear.CustomerIntelligence.Domain;
using NuClear.CustomerIntelligence.Querying.Tests.Model.CustomerIntelligence;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Processors;
using NuClear.Metamodeling.Provider;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.Querying.EntityFramework.Building;

using NUnit.Framework;

namespace NuClear.CustomerIntelligence.Querying.Tests
{
    public class EdmxBuilderBaseFixture
    {
        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            EffortProviderConfiguration.RegisterProvider();
        }

        protected static DbProviderInfo EffortProvider
        {
            get
            {
                return new DbProviderInfo(EffortProviderConfiguration.ProviderInvariantName, EffortProviderManifestTokens.Version1);
            }
        }

        protected static IMetadataSource AdvancedSearchMetadataSource
        {
            get
            {
                return new QueryingMetadataSource();
            }
        }

        protected static ITypeProvider CustomerIntelligenceTypeProvider
        {
            get
            {
                return MockTypeProvider(
                    typeof(CategoryGroup),
                    typeof(Client),
                    typeof(ClientContact),
                    typeof(Firm),
                    typeof(FirmBalance),
                    typeof(FirmCategory),
                    typeof(FirmTerritory),
                    typeof(Project),
                    typeof(Category),
                    typeof(Territory));
            }
        }

        private static ITypeProvider MockTypeProvider(params Type[] types)
        {
            var typeProvider = new Mock<ITypeProvider>();

            foreach (var type in types)
            {
                RegisterType(typeProvider, type);
            }

            return typeProvider.Object;
        }

        private static void RegisterType(Mock<ITypeProvider> typeProvider, Type type)
        {
            typeProvider.Setup(x => x.Resolve(It.Is<EntityElement>(el => el.ResolveName() == type.Name))).Returns(type);
        }

        protected static DbModel BuildModel(IMetadataProvider metadataProvider, ITypeProvider typeProvider = null)
        {
            var builder = CreateBuilder(metadataProvider, typeProvider);
            var contextId = BuildContextId();
            return builder.Build(EffortProvider, contextId);
        }

        protected static DbModel BuildModel(IMetadataSource source, ITypeProvider typeProvider = null)
        {
            return BuildModel(CreateMetadataProvider(source),  typeProvider);
        }

        protected static DbModel BuildModel(BoundedContextElement context, ITypeProvider typeProvider = null)
        {
            var builder = CreateBuilder(CreateMetadataProvider(MockSource(context)), typeProvider);
            var contextId = context.Identity.Id;
            var model = builder.Build(EffortProvider, contextId);

            model.Dump();

            return model;
        }

        // NOTE: Assembly name CANNOT start with digits, for example, 2GIS.Assembly.Name. It should be just, for example, Assembly.Name or DoubleGIS.Assembly.Name
        private static readonly string DefaultTestDataUri = string.Format("res://{0}/Data", Assembly.GetExecutingAssembly().GetName().Name);

        protected static DbConnection CreateConnection(string path = null)
        {
            var dataLoader = new CsvDataLoader(path ?? DefaultTestDataUri);
            var cachingLoader = new CachingDataLoader(dataLoader);
            return DbConnectionFactory.CreateTransient(cachingLoader);
        }

        private static EdmxModelBuilder CreateBuilder(IMetadataProvider metadataProvider, ITypeProvider typeProvider = null)
        {
            return typeProvider == null
                ? new EdmxModelBuilder(metadataProvider)
                : new EdmxModelBuilder(metadataProvider, typeProvider);
        }

        protected static Uri BuildContextId()
        {
            return QueryingMetadataIdentity.Instance.Id.WithRelative(new Uri("CustomerIntelligence", UriKind.Relative));
        }

        protected static IMetadataProvider CreateMetadataProvider(params IMetadataSource[] sources)
        {
            return new MetadataProvider(sources, new IMetadataProcessor[0]);
        }

        private static IMetadataSource MockSource(IMetadataElement context)
        {
            var source = new Mock<IMetadataSource>();
            source.Setup(x => x.Kind).Returns(new QueryingMetadataIdentity());
            source.Setup(x => x.Metadata).Returns(new Dictionary<Uri, IMetadataElement> { { Metadata.Id.For<QueryingMetadataIdentity>(), context } });

            return source.Object;
        }
    }
}