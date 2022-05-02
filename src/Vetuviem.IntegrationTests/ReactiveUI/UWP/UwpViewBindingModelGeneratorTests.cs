// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using ReactiveUI.UWP.VetuviemGenerator;
using Vetuviem.SourceGenerator.Features.ControlBindingModels;
using Vetuviem.Testing;
using Xunit.Abstractions;

namespace Vetuviem.IntegrationTests.ReactiveUI.UWP
{
    /// <summary>
    /// Unit Tests for the ViewBinding Model Source Generator.
    /// </summary>
    public static class UwpViewBindingModelGeneratorTests
    {
        /// <inheritdoc />
        public sealed class ExecuteMethod : BaseGeneratorTests.BaseExecuteMethod<UwpControlBindingModelGenerator, ControlBindingModelGeneratorProcessor>
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
            protected override void AddReferenceAssemblies(IList<MetadataReference> metadataReferences)
            {
                if (metadataReferences == null)
                {
                    throw new ArgumentNullException(nameof(metadataReferences));
                }

                metadataReferences.Add(MetadataReference.CreateFromFile(@"C:\Program Files (x86)\Windows Kits\10\UnionMetadata\10.0.16299.0\Windows.winmd"));
            }

            /// <inheritdoc />
            protected override Func<UwpControlBindingModelGenerator> GetFactory()
            {
                return () => new UwpControlBindingModelGenerator();
            }
        }
    }
}
