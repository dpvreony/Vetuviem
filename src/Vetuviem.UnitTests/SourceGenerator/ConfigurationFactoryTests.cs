// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Vetuviem.SourceGenerator.Features.Configuration;
using Vetuviem.Testing;
using Xunit;

namespace Vetuviem.UnitTests.SourceGenerator
{
    /// <summary>
    /// Unit Tests for the Configuration Factory.
    /// </summary>
    public static class ConfigurationFactoryTests
    {
        /// <summary>
        /// Unit Tests for the Create Method.
        /// </summary>
        public sealed class CreateMethod
        {
            [Fact]
            public void AllowExperimentalProperties_WhenTrue_SetsPropertyToTrue()
            {
                // Arrange
                var options = new InMemoryAnalyzerConfigOptions();
                options.Add("build_property.Vetuviem_Allow_Experimental_Properties", "true");
                var provider = new InMemoryAnalyzerConfigOptionsProvider(options);

                // Act
                var result = ConfigurationFactory.Create(provider);

                // Assert
                Assert.True(result.AllowExperimentalProperties);
            }

            [Fact]
            public void AllowExperimentalProperties_WhenFalse_SetsPropertyToFalse()
            {
                // Arrange
                var options = new InMemoryAnalyzerConfigOptions();
                options.Add("build_property.Vetuviem_Allow_Experimental_Properties", "false");
                var provider = new InMemoryAnalyzerConfigOptionsProvider(options);

                // Act
                var result = ConfigurationFactory.Create(provider);

                // Assert
                Assert.False(result.AllowExperimentalProperties);
            }

            [Fact]
            public void AllowExperimentalProperties_WhenNotSet_DefaultsToFalse()
            {
                // Arrange
                var options = new InMemoryAnalyzerConfigOptions();
                var provider = new InMemoryAnalyzerConfigOptionsProvider(options);

                // Act
                var result = ConfigurationFactory.Create(provider);

                // Assert
                Assert.False(result.AllowExperimentalProperties);
            }
        }
    }
}
