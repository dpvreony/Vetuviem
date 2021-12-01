using Microsoft.CodeAnalysis;
using Vetuviem.SourceGenerator;
using Vetuviem.SourceGenerator.Features.Core;

namespace ReactiveUI.Blazor.VetuviemGenerator
{
    [Generator]
    public sealed class BlazorControlBindingModelGenerator : AbstractControlBindingModelGenerator
    {
        protected override MetadataReference? CheckIfShouldAddMissingAssemblyReference(string assemblyOfInterest)
        {
            return null;
        }

        protected override IPlatformResolver GetPlatformResolver()
        {
            return new BlazorPlatformResolver();
        }

        protected override string GetPlatformName()
        {
            return "Blazor";
        }
    }
}
