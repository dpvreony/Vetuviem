using System;
using Vetuviem.SourceGenerator.Features.Core;

namespace ReactiveUI.WPF.VetuviemGenerator
{
    public sealed class WpfPlatformResolver : IPlatformResolver
    {
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

        public string GetBaseUiElement()
        {
            return "System.Windows.UIElement";
        }

        public string GetCommandInterface()
        {
            return "global::System.Windows.Input.ICommand";
        }
    }
}
