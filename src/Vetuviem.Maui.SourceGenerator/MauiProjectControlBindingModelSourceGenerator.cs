// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Vetuviem.SourceGenerator;
using Vetuviem.SourceGenerator.Features.Core;

namespace Vetuviem.Maui.SourceGenerator
{
    /// <summary>
    /// Project level Control Binding Model Source Generator for WPF.
    /// </summary>
    [Generator]
    public sealed class MauiProjectControlBindingModelSourceGenerator : AbstractProjectControlBindingModelSourceGenerator
    {
        /// <inheritdoc />
        protected override string GetPlatformName() => "Maui";

        /// <inheritdoc/>
        protected override IPlatformResolver GetPlatformResolver() => new MauiPlatformResolver();
    }
}
