using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using ReactiveUI.UWP.VetuviemGenerator;
using Vetuviem.SourceGenerator.Features.ViewBindingModels;
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
        public sealed class ExecuteMethod : BaseGeneratorTests.BaseExecuteMethod<UwpViewBindingModelGenerator, ViewBindingModelGeneratorProcessor>
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
                metadataReferences.Add(MetadataReference.CreateFromFile(@"C:\Program Files (x86)\Windows Kits\10\UnionMetadata\10.0.16299.0\Windows.winmd"));
            }

            /// <inheritdoc />
            protected override Func<UwpViewBindingModelGenerator> GetFactory()
            {
                return () => new UwpViewBindingModelGenerator();
            }
        }
    }
}
