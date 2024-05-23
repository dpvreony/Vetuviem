// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Vetuviem.SourceGenerator.Features.Core;

namespace Vetuviem.Winforms.SourceGenerator
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
        public string? GetCommandInterface()
        {
            return null;
        }
    }
}
