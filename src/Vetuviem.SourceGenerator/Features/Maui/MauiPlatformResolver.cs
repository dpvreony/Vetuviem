// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Vetuviem.SourceGenerator.Features.Core;

namespace Vetuviem.SourceGenerator.Features.Maui
{
    public sealed class MauiPlatformResolver : IPlatformResolver
    {
        /// <inheritdoc />
        public string[] GetAssemblyNames()
        {
            return new[]
            {
                "Microsoft.Maui.Controls.dll",
                "ReactiveUI.Maui.dll",
            };
        }

        /// <inheritdoc />
        public string GetBaseUiElement()
        {
            return "global::Microsoft.Maui.Controls.Element";
        }

        /// <inheritdoc />
        public string GetCommandSourceInterface()
        {
            return "global::System.Windows.Input.ICommandSource";
        }

        /// <inheritdoc />
        public string GetCommandInterface()
        {
            return "global::System.Windows.Input.ICommand";
        }
    }
}
