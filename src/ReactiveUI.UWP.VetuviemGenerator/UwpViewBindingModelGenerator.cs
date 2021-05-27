using Microsoft.CodeAnalysis;
using Vetuviem.SourceGenerator;
using Vetuviem.SourceGenerator.Features.Core;

namespace ReactiveUI.UWP.VetuviemGenerator
{
    /// <summary>
    /// Source Generator for UWP View Binding Models.
    /// </summary>
    [Generator]
    public sealed class UwpViewBindingModelGenerator : AbstractViewBindingModelGenerator
    {
        /// <inheritdoc />
        protected override IPlatformResolver GetPlatformResolver()
        {
            return new UwpPlatformResolver();
        }
    }
}
