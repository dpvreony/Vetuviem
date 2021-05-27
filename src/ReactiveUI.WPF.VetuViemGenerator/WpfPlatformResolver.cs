using Vetuviem.SourceGenerator.Features.Core;

namespace ReactiveUI.WPF.VetuviemGenerator
{
    /// <summary>
    /// UI Platform resolver for WPF.
    /// </summary>
    public sealed class WpfPlatformResolver : IPlatformResolver
    {
        /// <inheritdoc />
        public string[] GetAssemblyNames()
        {
            return new[]
            {
                "PresentationCore.dll",
                "PresentationFramework.dll",
                "PresentationFramework.Aero.dll",
                "PresentationFramework.Aero2.dll",
                "PresentationFramework.AeroLite.dll",
                "PresentationFramework.Classic.dll",
                "PresentationFramework.Luna.dll",
                "PresentationFramework.Royale.dll",
                "PresentationUI.dll",
            };
        }

        /// <inheritdoc />
        public string GetBaseUiElement()
        {
            return "global::System.Windows.UIElement";
        }

        /// <inheritdoc />
        public string GetCommandInterface()
        {
            return "global::System.Windows.Input.ICommand";
        }
    }
}
