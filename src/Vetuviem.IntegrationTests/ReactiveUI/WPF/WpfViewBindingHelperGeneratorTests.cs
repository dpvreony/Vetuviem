using System;
using System.Collections.Generic;
using System.IO;
using Dhgms.Nucleotide.UnitTests.Generators;
using Microsoft.CodeAnalysis;
using ReactiveUI.WPF.VetuviemGenerator;
using Vetuviem.SourceGenerator.GeneratorProcessors;
using Xunit.Abstractions;

namespace Vetuviem.IntegrationTests
{
    /// <summary>
    /// Unit Tests for the ViewBinding Helper Source Generator.
    /// </summary>
    public static class WpfViewBindingHelperGeneratorTests
    {
        public sealed class ExecuteMethod : BaseGeneratorTests.BaseExecuteMethod<WpfViewBindingHelperGenerator, ViewBindingHelperGeneratorProcessor>
        {
            public ExecuteMethod(ITestOutputHelper output)
                : base(output)
            {
            }

            protected override void AddReferenceAssemblies(List<MetadataReference> metadataReferences)
            {
                var trustedAssembliesPaths = GetPlatformAssemblyPaths();
                foreach (string trustedAssembliesPath in trustedAssembliesPaths)
                {
                    var metadataReference = MetadataReference.CreateFromFile(trustedAssembliesPath);
                    metadataReferences.Add(metadataReference);
                }
            }

            protected override Func<WpfViewBindingHelperGenerator> GetFactory()
            {
                return () => new WpfViewBindingHelperGenerator();
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
