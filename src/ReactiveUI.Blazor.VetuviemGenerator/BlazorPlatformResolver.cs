using Vetuviem.SourceGenerator.Features.Core;

namespace ReactiveUI.Blazor.VetuviemGenerator
{
    /// <summary>
    /// UI Platform resolver for Blazor.
    /// </summary>
    public sealed class BlazorPlatformResolver : IPlatformResolver
    {
        /// <inheritdoc />
        public string[] GetAssemblyNames()
        {
            return new[]
            {
                "Microsoft.AspNetCore.Components.dll",
                "Microsoft.AspNetCore.Components.Forms.dll",
                "Microsoft.AspNetCore.Components.Web.dll",
            };
        }

        /// <inheritdoc />
        public string GetBaseUiElement()
        {
            return "global::Microsoft.AspNetCore.Components.IComponent";
        }

        /// <inheritdoc />
        public string GetCommandInterface()
        {
            return null;
        }
    }
}
