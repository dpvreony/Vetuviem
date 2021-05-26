﻿using Vetuviem.SourceGenerator.Features.Core;

namespace ReactiveUI.WinUI3.VetuviemGenerator
{
    public sealed class WinUi3PlatformResolver : IPlatformResolver
    {
        public string[] GetAssemblyNames()
        {
            return new[]
            {
                "Microsoft.WinUI.dll",
            };
        }

        public string GetBaseUiElement()
        {
            return "Microsoft.WinUI.Xaml.UIElement";
        }

        public string GetCommandInterface()
        {
            return "System.Windows.Input.ICommand";
        }
    }
}
