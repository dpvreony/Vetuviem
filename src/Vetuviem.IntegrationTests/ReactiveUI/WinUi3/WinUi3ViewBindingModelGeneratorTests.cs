using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using ReactiveUI.WinUI3.VetuviemGenerator;
using Vetuviem.SourceGenerator.Features.ControlBindingModels;
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
        public sealed class ExecuteMethod : BaseGeneratorTests.BaseExecuteMethod<WinUi3ControlBindingModelGenerator, ControlBindingModelGeneratorProcessor>
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

                metadataReferences.Add(MetadataReference.CreateFromFile("Microsoft.WinUI.dll"));
            }

            /// <inheritdoc />
            protected override Func<WinUi3ControlBindingModelGenerator> GetFactory()
            {
                return () => new WinUi3ControlBindingModelGenerator();
            }
        }
    }
}
