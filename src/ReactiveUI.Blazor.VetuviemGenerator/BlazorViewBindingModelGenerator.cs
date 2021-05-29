using Microsoft.CodeAnalysis;
using Vetuviem.SourceGenerator;
using Vetuviem.SourceGenerator.Features.Core;

namespace ReactiveUI.Blazor.VetuviemGenerator
{
    [Generator]
    public sealed class BlazorViewBindingModelGenerator : AbstractViewBindingModelGenerator
    {
        protected override MetadataReference CheckIfShouldAddMissingAssemblyReference(string assemblyOfInterest)
        {
            return null;
        }

        protected override IPlatformResolver GetPlatformResolver()
        {
            return new BlazorPlatformResolver();
        }
    }
}
