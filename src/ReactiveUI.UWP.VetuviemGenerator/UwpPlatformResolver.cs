// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Vetuviem.SourceGenerator.Features.Core;

namespace ReactiveUI.UWP.VetuviemGenerator
{
    /// <summary>
    /// UI Platform resolver for UWP.
    /// </summary>
    public sealed class UwpPlatformResolver : IPlatformResolver
    {
        /// <inheritdoc />
        public string[] GetAssemblyNames()
        {
            return new[]
            {
                @"C:\Program Files (x86)\Windows Kits\10\UnionMetadata\10.0.16299.0\Windows.winmd",
            };
        }

        /// <inheritdoc />
        public string GetBaseUiElement()
        {
            return "global::Windows.UI.Xaml.UIElement";
        }

        /// <inheritdoc />
        public string GetCommandInterface()
        {
            return "global::Windows.UI.Xaml.Input.ICommand";
        }
    }
}
