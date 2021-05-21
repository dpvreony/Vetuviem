using System;
using Dhgms.Nucleotide.UnitTests.Generators;
using ReactiveUI.WPF.ViewToViewModelBindings;
using Vetuviem.SourceGenerator.GeneratorProcessors;
using Xunit.Abstractions;

namespace Vetuviem.IntegrationTests
{
    /// <summary>
    /// Unit Tests for the ViewBinding Helper Source Generator.
    /// </summary>
    public static class ViewBindingHelperGeneratorTests
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
