using Microsoft.CodeAnalysis;
using Vetuviem.SourceGenerator;
using Vetuviem.SourceGenerator.Features.Core;

namespace ReactiveUI.WPF.VetuviemGenerator
{
    [Generator]
    public sealed class WpfViewBindingHelperGenerator : AbstractViewBindingHelperGenerator
    {
        protected override IPlatformResolver GetPlatformResolver()
        {
            return new WpfPlatformResolver();
        }
    }
}
