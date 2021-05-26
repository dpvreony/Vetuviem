using Microsoft.CodeAnalysis;
using Vetuviem.SourceGenerator;
using Vetuviem.SourceGenerator.Features.Core;

namespace ReactiveUI.UWP.VetuviemGenerator
{
    [Generator]
    public sealed class UwpViewBindingModelGenerator : AbstractViewBindingModelGenerator
    {
        protected override IPlatformResolver GetPlatformResolver()
        {
            return new UwpPlatformResolver();
        }
    }
}
