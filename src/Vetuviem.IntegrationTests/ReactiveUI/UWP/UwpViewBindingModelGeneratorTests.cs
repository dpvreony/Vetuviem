using System;
using System.Collections.Generic;
using Dhgms.Nucleotide.UnitTests.Generators;
using Microsoft.CodeAnalysis;
using ReactiveUI.UWP.VetuviemGenerator;
using Vetuviem.SourceGenerator.GeneratorProcessors;
using Xunit.Abstractions;

namespace Vetuviem.IntegrationTests
{
    /// <summary>
    /// Unit Tests for the ViewBinding Model Source Generator.
    /// </summary>
    public static class UwpViewBindingModelGeneratorTests
    {
        public sealed class ExecuteMethod : BaseGeneratorTests.BaseExecuteMethod<UwpViewBindingModelGenerator, ViewBindingModelGeneratorProcessor>
        {
            public ExecuteMethod(ITestOutputHelper output)
                : base(output)
            {
            }

            protected override void AddReferenceAssemblies(List<MetadataReference> metadataReferences)
            {
                metadataReferences.Add(MetadataReference.CreateFromFile(@"C:\Program Files (x86)\Windows Kits\10\UnionMetadata\10.0.16299.0\Windows.winmd"));
            }

            protected override Func<UwpViewBindingModelGenerator> GetFactory()
            {
                return () => new UwpViewBindingModelGenerator();
            }
        }
    }
}
