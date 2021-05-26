using System;
using System.Collections.Generic;
using Dhgms.Nucleotide.UnitTests.Generators;
using Microsoft.CodeAnalysis;
using ReactiveUI.WinUI3.VetuviemGenerator;
using ReactiveUI.WPF.VetuviemGenerator;
using Vetuviem.SourceGenerator.GeneratorProcessors;
using Xunit.Abstractions;

namespace Vetuviem.IntegrationTests
{
    /// <summary>
    /// Unit Tests for the ViewBinding Model Source Generator.
    /// </summary>
    public static class ViewBindingModelGeneratorTests
    {
        public sealed class ExecuteMethod : BaseGeneratorTests.BaseExecuteMethod<WinUi3ViewBindingModelGenerator, ViewBindingModelGeneratorProcessor>
        {
            public ExecuteMethod(ITestOutputHelper output)
                : base(output)
            {
            }

            protected override void AddReferenceAssemblies(List<MetadataReference> metadataReferences)
            {
                metadataReferences.Add(MetadataReference.CreateFromFile("Microsoft.WinUI.dll"));
            }

            protected override Func<WinUi3ViewBindingModelGenerator> GetFactory()
            {
                return () => new WinUi3ViewBindingModelGenerator();
            }
        }
    }
}
