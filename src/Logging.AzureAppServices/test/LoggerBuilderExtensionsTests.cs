// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Microsoft.Extensions.Logging.AzureAppServices.Test
{
    public class LoggerBuilderExtensionsTests
    {
        private IWebAppContext _appContext;

        public LoggerBuilderExtensionsTests()
        {
            var contextMock = new Mock<IWebAppContext>();
            contextMock.SetupGet(c => c.IsRunningInAzureWebApp).Returns(true);
            contextMock.SetupGet(c => c.HomeFolder).Returns(".");
            _appContext = contextMock.Object;
        }

        [Fact]
        public void BuilderExtensionAddsSingleSetOfServicesWhenCalledTwiceOrig()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder => builder.AddAzureWebAppDiagnostics(_appContext, null));
            var count = serviceCollection.Count;

            Assert.NotEqual(0, count);

            serviceCollection.AddLogging(builder => builder.AddAzureWebAppDiagnostics(_appContext, null));

            Assert.Equal(count, serviceCollection.Count);
        }

        [Fact]
        public void BuilderExtensionAddsSingleSetOfServicesWhenCalledTwiceOverload()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder => builder.AddAzureWebAppDiagnostics(_appContext, "customPrefix"));
            var count = serviceCollection.Count;

            Assert.NotEqual(0, count);

            serviceCollection.AddLogging(builder => builder.AddAzureWebAppDiagnostics(_appContext, "customPrefix"));

            Assert.Equal(count, serviceCollection.Count);
        }

        [Fact]
        public void BuilderExtensionAddsConfigurationChangeTokenSourceOrig()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder => builder.AddConfiguration(new ConfigurationBuilder().Build()));

            // Tracking for main configuration
            Assert.Equal(1, serviceCollection.Count(d => d.ServiceType == typeof(IOptionsChangeTokenSource<LoggerFilterOptions>)));

            serviceCollection.AddLogging(builder => builder.AddAzureWebAppDiagnostics(_appContext, null));

            // Make sure we add another config change token for azure diagnostic configuration
            Assert.Equal(2, serviceCollection.Count(d => d.ServiceType == typeof(IOptionsChangeTokenSource<LoggerFilterOptions>)));
        }

        [Fact]
        public void BuilderExtensionAddsConfigurationChangeTokenSourceOverload()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder => builder.AddConfiguration(new ConfigurationBuilder().Build()));

            // Tracking for main configuration
            Assert.Equal(1, serviceCollection.Count(d => d.ServiceType == typeof(IOptionsChangeTokenSource<LoggerFilterOptions>)));

            serviceCollection.AddLogging(builder => builder.AddAzureWebAppDiagnostics(_appContext, "customPrefix"));

            // Make sure we add another config change token for azure diagnostic configuration
            Assert.Equal(2, serviceCollection.Count(d => d.ServiceType == typeof(IOptionsChangeTokenSource<LoggerFilterOptions>)));
        }

        [Fact]
        public void BuilderExtensionAddsIConfigureOptionsOrig()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder => builder.AddConfiguration(new ConfigurationBuilder().Build()));

            // Tracking for main configuration
            Assert.Equal(2, serviceCollection.Count(d => d.ServiceType == typeof(IConfigureOptions<LoggerFilterOptions>)));

            serviceCollection.AddLogging(builder => builder.AddAzureWebAppDiagnostics(_appContext, null));

            Assert.Equal(4, serviceCollection.Count(d => d.ServiceType == typeof(IConfigureOptions<LoggerFilterOptions>)));
        }

        [Fact]
        public void BuilderExtensionAddsIConfigureOptionsOverload()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder => builder.AddConfiguration(new ConfigurationBuilder().Build()));

            // Tracking for main configuration
            Assert.Equal(2, serviceCollection.Count(d => d.ServiceType == typeof(IConfigureOptions<LoggerFilterOptions>)));

            serviceCollection.AddLogging(builder => builder.AddAzureWebAppDiagnostics(_appContext, "customPrefix"));

            Assert.Equal(4, serviceCollection.Count(d => d.ServiceType == typeof(IConfigureOptions<LoggerFilterOptions>)));
        }

        [Fact]
        public void LoggerProviderIsResolvableOrig()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder => builder.AddAzureWebAppDiagnostics(_appContext, null));

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var loggerFactory = serviceProvider.GetService<ILoggerProvider>();
        }

        [Fact]
        public void LoggerProviderIsResolvableOverload()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder => builder.AddAzureWebAppDiagnostics(_appContext, "customPrefix"));

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var loggerFactory = serviceProvider.GetService<ILoggerProvider>();
        }
    }
}
