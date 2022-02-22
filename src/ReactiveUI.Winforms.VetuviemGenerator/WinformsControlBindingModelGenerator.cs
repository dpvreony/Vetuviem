using Microsoft.CodeAnalysis;
using Vetuviem.SourceGenerator;
using Vetuviem.SourceGenerator.Features.Core;

namespace ReactiveUI.Winforms.VetuviemGenerator
{
    [Generator]
    public sealed class WinformsControlBindingModelGenerator : AbstractControlBindingModelGenerator
    {
        /// <inheritdoc />
        protected override MetadataReference? CheckIfShouldAddMissingAssemblyReference(string assemblyOfInterest)
        {
            return null;
        }

        /// <inheritdoc />
        protected override string GetPlatformName()
        {
            return "Winforms";
        }

        /// <inheritdoc />
        protected override IPlatformResolver GetPlatformResolver()
        {
            return new WinformsPlatformResolver();
        }
    }
}
