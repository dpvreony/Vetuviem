using Microsoft.CodeAnalysis;
using Vetuviem.SourceGenerator;
using Vetuviem.SourceGenerator.Features.Core;

namespace ReactiveUI.Winforms.VetuviemGenerator
{
    [Generator]
    public sealed class WinformsViewBindingModelGenerator : AbstractViewBindingModelGenerator
    {
        protected override MetadataReference CheckIfShouldAddMissingAssemblyReference(string assemblyOfInterest)
        {
            return null;
        }

        protected override string GetPlatformName()
        {
            return "Winforms";
        }

        protected override IPlatformResolver GetPlatformResolver()
        {
            return new WinformsPlatformResolver();
        }
    }
}
