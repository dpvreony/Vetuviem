using Microsoft.CodeAnalysis;
using Vetuviem.SourceGenerator;
using Vetuviem.SourceGenerator.Features.Core;

namespace ReactiveUI.WinUI3.VetuviemGenerator
{
    [Generator]
    public sealed class WinUi3ViewBindingModelGenerator : AbstractViewBindingModelGenerator
    {
        protected override IPlatformResolver GetPlatformResolver()
        {
            return new WinUi3PlatformResolver();
        }
    }
}
