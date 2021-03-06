using Microsoft.CodeAnalysis;
using Vetuviem.SourceGenerator;
using Vetuviem.SourceGenerator.Features.Core;

namespace ReactiveUI.WPF.VetuviemGenerator
{
    [Generator]
    public sealed class WpfViewBindingModelGenerator : AbstractViewBindingModelGenerator
    {
        protected override MetadataReference CheckIfShouldAddMissingAssemblyReference(string assemblyOfInterest)
        {
            return null;
        }

        protected override string GetPlatformName()
        {
            return "Wpf";
        }

        protected override IPlatformResolver GetPlatformResolver()
        {
            return new WpfPlatformResolver();
        }
    }
}
