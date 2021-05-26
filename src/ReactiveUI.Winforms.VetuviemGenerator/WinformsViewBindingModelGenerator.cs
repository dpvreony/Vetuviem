using Microsoft.CodeAnalysis;
using Vetuviem.SourceGenerator;
using Vetuviem.SourceGenerator.Features.Core;

namespace ReactiveUI.Winforms.VetuviemGenerator
{
    [Generator]
    public sealed class WinformsViewBindingModelGenerator : AbstractViewBindingModelGenerator
    {
        protected override IPlatformResolver GetPlatformResolver()
        {
            return new WinformsPlatformResolver();
        }
    }
}
