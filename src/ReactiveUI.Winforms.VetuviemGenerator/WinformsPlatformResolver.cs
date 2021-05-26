using Vetuviem.SourceGenerator.Features.Core;

namespace ReactiveUI.Winforms.VetuviemGenerator
{
    public sealed class WinformsPlatformResolver : IPlatformResolver
    {
        public string[] GetAssemblyNames()
        {
            return new[]
            {
                "System.Windows.Forms.dll",
            };
        }

        public string GetBaseUiElement()
        {
            return "global::System.Windows.Forms.Control";
        }

        public string GetCommandInterface()
        {
            return null;
        }
    }
}
