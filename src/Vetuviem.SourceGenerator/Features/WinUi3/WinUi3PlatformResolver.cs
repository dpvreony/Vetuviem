// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Vetuviem.SourceGenerator.Features.Core;

namespace ReactiveUI.WinUI3.VetuviemGenerator
{
    /// <summary>
    /// UI Platform resolver for WinUI3.
    /// </summary>
    public sealed class WinUi3PlatformResolver : IPlatformResolver
    {
        /// <inheritdoc />
        public string[] GetAssemblyNames()
        {
            return new[]
            {
                "Microsoft.WinUI.dll",
            };
        }

        /// <inheritdoc />
        public string GetBaseUiElement()
        {
            return "global::Microsoft.UI.Xaml.UIElement";
        }

        /// <inheritdoc />
        public string GetCommandInterface()
        {
            return "global::System.Windows.Input.ICommand";
        }
    }
}
