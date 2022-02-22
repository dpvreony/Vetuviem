using Microsoft.CodeAnalysis;
using Vetuviem.SourceGenerator;
using Vetuviem.SourceGenerator.Features.Core;

namespace ReactiveUI.Blazor.VetuviemGenerator
{
    [Generator]
    public sealed class BlazorControlBindingModelGenerator : AbstractControlBindingModelGenerator
    {
        /// <inheritdoc />
        protected override MetadataReference? CheckIfShouldAddMissingAssemblyReference(string assemblyOfInterest)
        {
            return null;
        }

        /// <inheritdoc />
        protected override IPlatformResolver GetPlatformResolver()
        {
            return new BlazorPlatformResolver();
        }

        /// <inheritdoc />
        protected override string GetPlatformName()
        {
            return "Blazor";
        }
    }
}
