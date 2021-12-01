using Microsoft.CodeAnalysis;
using Vetuviem.SourceGenerator;
using Vetuviem.SourceGenerator.Features.Core;

namespace ReactiveUI.UWP.VetuviemGenerator
{
    /// <summary>
    /// Source Generator for UWP View Binding Models.
    /// </summary>
    [Generator]
    public sealed class UwpControlBindingModelGenerator : AbstractControlBindingModelGenerator
    {
        protected override MetadataReference CheckIfShouldAddMissingAssemblyReference(string assemblyOfInterest)
        {
            return MetadataReference.CreateFromFile(assemblyOfInterest);
        }

        /// <inheritdoc />
        protected override IPlatformResolver GetPlatformResolver()
        {
            return new UwpPlatformResolver();
        }

        protected override string GetPlatformName()
        {
            return "UWP";
        }
    }
}
