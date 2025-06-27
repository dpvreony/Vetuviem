// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Vetuviem.SourceGenerator;
using Vetuviem.SourceGenerator.Features.Core;
using Vetuviem.WinUi.SourceGenerator;

namespace Vetuviem.WPF.SourceGenerator
{
    /// <summary>
    /// Project level Control Binding Model Source Generator for WPF.
    /// </summary>
    [Generator]
    public sealed class WinUI3ProjectControlBindingModelSourceGenerator : AbstractProjectControlBindingModelSourceGenerator
    {
        /// <inheritdoc />
        protected override string GetPlatformName() => "WinUi3";

        /// <inheritdoc/>
        protected override IPlatformResolver GetPlatformResolver() => new WinUi3PlatformResolver();
    }
}
