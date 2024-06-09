// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Vetuviem.SourceGenerator.Features.Core;

namespace Vetuviem.Blazor.SourceGenerator
{
    /// <summary>
    /// UI Platform resolver for Blazor.
    /// </summary>
    public sealed class BlazorPlatformResolver : IPlatformResolver
    {
        /// <inheritdoc />
        public string[] GetAssemblyNames()
        {
            return new[]
            {
                "Microsoft.AspNetCore.Components.dll",
                "Microsoft.AspNetCore.Components.Forms.dll",
                "Microsoft.AspNetCore.Components.Web.dll",
                "ReactiveUI.Blazor.dll",
            };
        }

        /// <inheritdoc />
        public string GetBaseUiElement()
        {
            return "global::Microsoft.AspNetCore.Components.IComponent";
        }

        /// <inheritdoc />
        public string? GetCommandSourceInterface()
        {
            return null;
        }

        /// <inheritdoc />
        public string? GetCommandInterface()
        {
            return null;
        }
    }
}
