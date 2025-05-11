// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Vetuviem.SourceGenerator.Features.ControlBindingModels;
using Vetuviem.Testing;
using Vetuviem.WinUi.SourceGenerator;
using Xunit.Abstractions;

namespace Vetuviem.IntegrationTests.ReactiveUI.WinUi3
{
    /// <summary>
    /// Unit Tests for the ViewBinding Model Source Generator.
    /// </summary>
    public static class WinUi3ViewBindingModelGeneratorTests
    {
        /// <inheritdoc />
        public sealed class ExecuteMethod : BaseGeneratorTests.BaseExecuteMethod<WinUi3ControlBindingModelSourceGenerator, ControlBindingModelGeneratorProcessor>
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
                return null;
            }

            /// <inheritdoc />
            protected override void AddReferenceAssemblies(IList<MetadataReference> metadataReferences)
            {
                ArgumentNullException.ThrowIfNull(metadataReferences);

                metadataReferences.Add(MetadataReference.CreateFromFile("Microsoft.WinUI.dll"));
            }

            /// <inheritdoc />
            protected override Func<WinUi3ControlBindingModelSourceGenerator> GetFactory()
            {
                return () => new WinUi3ControlBindingModelSourceGenerator();
            }
        }
    }
}
