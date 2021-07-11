using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using ReactiveUI.WinUI3.VetuviemGenerator;
using Vetuviem.SourceGenerator.Features.ViewBindingModels;
using Vetuviem.Testing;
using Xunit.Abstractions;

namespace Vetuviem.IntegrationTests.ReactiveUI.WinUi3
{
    /// <summary>
    /// Unit Tests for the ViewBinding Model Source Generator.
    /// </summary>
    public static class ViewBindingModelGeneratorTests
    {
        /// <inheritdoc />
        public sealed class ExecuteMethod : BaseGeneratorTests.BaseExecuteMethod<WinUi3ViewBindingModelGenerator, ViewBindingModelGeneratorProcessor>
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
            protected override void AddReferenceAssemblies(List<MetadataReference> metadataReferences)
            {
                metadataReferences.Add(MetadataReference.CreateFromFile("Microsoft.WinUI.dll"));
            }

            /// <inheritdoc />
            protected override Func<WinUi3ViewBindingModelGenerator> GetFactory()
            {
                return () => new WinUi3ViewBindingModelGenerator();
            }
        }
    }
}
