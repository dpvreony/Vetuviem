using System;
using Dhgms.Nucleotide.UnitTests.Generators;
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
        public sealed class ExecuteMethod : BaseGeneratorTests.BaseExecuteMethod<WpfViewBindingModelGenerator, ViewBindingModelGeneratorProcessor>
        {
            public ExecuteMethod(ITestOutputHelper output)
                : base(output)
            {
            }

            protected override Func<WpfViewBindingModelGenerator> GetFactory()
            {
                return () => new WpfViewBindingModelGenerator();
            }
        }
    }
}
