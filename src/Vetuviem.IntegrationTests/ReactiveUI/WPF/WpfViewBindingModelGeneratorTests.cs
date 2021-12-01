using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis;
using ReactiveUI.WPF.VetuviemGenerator;
using Vetuviem.SourceGenerator.Features.ControlBindingModels;
using Vetuviem.Testing;
using Xunit.Abstractions;

namespace Vetuviem.IntegrationTests.ReactiveUI.WPF
{
    /// <summary>
    /// Unit Tests for the ViewBinding Model Source Generator.
    /// </summary>
    public static class WpfViewBindingModelGeneratorTests
    {
        /// <inheritdoc />
        public sealed class ExecuteMethod : BaseGeneratorTests.BaseExecuteMethod<WpfControlBindingModelGenerator, ControlBindingModelGeneratorProcessor>
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

            /// <inheritdoc />
            protected override Func<WpfControlBindingModelGenerator> GetFactory()
            {
                return () => new WpfControlBindingModelGenerator();
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
