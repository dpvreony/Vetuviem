using Vetuviem.SourceGenerator.Features.Core;

namespace ReactiveUI.UWP.VetuviemGenerator
{
    public sealed class UwpPlatformResolver : IPlatformResolver
    {
        public string[] GetAssemblyNames()
        {
            return new[]
            {
                @"C:\Program Files (x86)\Windows Kits\10\UnionMetadata\10.0.16299.0\Windows.winmd",
            };
        }

        public string GetBaseUiElement()
        {
            return "global::Windows.UI.Xaml.UIElement";
        }

        public string GetCommandInterface()
        {
            return "global::Windows.UI.Xaml.Input.ICommand";
        }
    }
}
