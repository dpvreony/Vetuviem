using Microsoft.CodeAnalysis;
using Vetuviem.SourceGenerator;
using Vetuviem.SourceGenerator.Features.Core;

namespace ReactiveUI.WinUI3.VetuviemGenerator
{
    [Generator]
    public sealed class WinUi3ViewBindingModelGenerator : AbstractViewBindingModelGenerator
    {
        protected override MetadataReference CheckIfShouldAddMissingAssemblyReference(string assemblyOfInterest)
        {
            return null;
        }

        protected override string GetPlatformName()
        {
            return "WinUi3";
        }

        protected override IPlatformResolver GetPlatformResolver()
        {
            return new WinUi3PlatformResolver();
        }
    }
}
