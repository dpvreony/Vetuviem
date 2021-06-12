using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis;
using ReactiveUI.Winforms.VetuviemGenerator;
using Vetuviem.SourceGenerator.Features.ViewBindingModels;
using Vetuviem.SourceGenerator.GeneratorProcessors;
using Vetuviem.Testing;
using Xunit.Abstractions;

namespace Vetuviem.IntegrationTests.ReactiveUI.Winforms
{
    /// <summary>
    /// Unit Tests for the ViewBinding Model Source Generator.
    /// </summary>
    public static class ViewBindingModelGeneratorTests
    {
        /// <inheritdoc />
        public sealed class ExecuteMethod : BaseGeneratorTests.BaseExecuteMethod<WinformsViewBindingModelGenerator, ViewBindingModelGeneratorProcessor, AbstractViewBindingModelClassGenerator>
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
                var trustedAssembliesPaths = GetPlatformAssemblyPaths();
                foreach (string trustedAssembliesPath in trustedAssembliesPaths)
                {
                    var metadataReference = MetadataReference.CreateFromFile(trustedAssembliesPath);
                    metadataReferences.Add(metadataReference);
                }
            }

            /// <inheritdoc />
            protected override Func<WinformsViewBindingModelGenerator> GetFactory()
            {
                return () => new WinformsViewBindingModelGenerator();
            }

            private static string[] GetPlatformAssemblyPaths()
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
