// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Vetuviem.SourceGenerator.Features.Core;

namespace Vetuviem.Avalonia.SourceGenerator
{
    /// <summary>
    /// UI Platform resolver for Blazor.
    /// </summary>
    public sealed class AvaloniaPlatformResolver : IPlatformResolver
    {
        /// <inheritdoc />
        public string[] GetAssemblyNames()
        {
            return new[]
            {
                "Avalonia.Base.dll",
                "Avalonia.Controls.dll",
                "Avalonia.ReactiveUI.dll"
            };
        }

        /// <inheritdoc />
        public string GetBaseUiElement()
        {
            return "global::Avalonia.Visual";
        }

        /// <inheritdoc />
        public string? GetCommandSourceInterface()
        {
            return "global::Avalonia.Input.ICommandSource";
        }

        /// <inheritdoc />
        public string GetCommandInterface()
        {
            return "global::Avalonia.Input.ICommand";
        }
    }
}
