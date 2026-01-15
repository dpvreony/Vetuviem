// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Vetuviem.SourceGenerator;
using Vetuviem.SourceGenerator.Features.ControlBindingModels;
using Vetuviem.Testing;
using Xunit;

namespace Vetuviem.IntegrationTests.ReactiveUI.Blazor
{
    /// <summary>
    /// Unit Tests for the ViewBinding Model Source Generator.
    /// </summary>
    public static class MudBlazorViewBindingModelGeneratorTests
    {
        /// <inheritdoc />
        public sealed class ExecuteMethod : BaseGeneratorTests.BaseExecuteMethod<VetuviemEntryAssemblySourceGenerator>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ExecuteMethod"/> class.
            /// </summary>
            /// <param name="output">Test Output Helper.</param>
            public ExecuteMethod(ITestOutputHelper output)
                : base(output)
            {
            }

            /// <inheritdoc />
            protected override string GetProjectSourceCode()
            {
                return string.Empty;
            }

            /// <inheritdoc />
            protected override AnalyzerConfigOptionsProvider? GetAnalyzerConfigOptionsProvider()
            {
                var globalOptions = new InMemoryAnalyzerConfigOptions();
                globalOptions.Add(
                    "build_property.Vetuviem_Assemblies",
                    "MudBlazor.dll");
                globalOptions.Add(
                    "build_property.Vetuviem_Assembly_Mode",
                    "Extend");
                globalOptions.Add(
                    "build_property.Vetuviem_Ui_Framework",
                    "Blazor");

                return new InMemoryAnalyzerConfigOptionsProvider(globalOptions);
            }

            /// <inheritdoc />
            protected override void AddReferenceAssemblies(IList<MetadataReference> metadataReferences)
            {
                ArgumentNullException.ThrowIfNull(metadataReferences);

                var trustedAssembliesPaths = GetPlatformAssemblyPaths();
                if (trustedAssembliesPaths == null)
                {
                    return;
                }

                foreach (string trustedAssembliesPath in trustedAssembliesPaths)
                {
                    var metadataReference = MetadataReference.CreateFromFile(trustedAssembliesPath);
                    metadataReferences.Add(metadataReference);
                }
            }

            private static string[]? GetPlatformAssemblyPaths()
            {
                if (AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") is string trustedPlatformAssemblies)
                {
                    return trustedPlatformAssemblies.Split(Path.PathSeparator);
                }

                return null;
            }
        }
    }
}
