using Vetuviem.SourceGenerator.Features.Core;

namespace ReactiveUI.Winforms.VetuviemGenerator
{
    /// <summary>
    /// UI Platform resolver for Windows Forms.
    /// </summary>
    public sealed class WinformsPlatformResolver : IPlatformResolver
    {
        /// <inheritdoc />
        public string[] GetAssemblyNames()
        {
            return new[]
            {
                "System.Windows.Forms.dll",
            };
        }

        /// <inheritdoc />
        public string GetBaseUiElement()
        {
            return "global::System.Windows.Forms.Control";
        }

        /// <inheritdoc />
        public string GetCommandInterface()
        {
            return null;
        }
    }
}
