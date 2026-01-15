// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Vetuviem.SourceGenerator;
using Vetuviem.SourceGenerator.Features.Core;

namespace Vetuviem.Avalonia.SourceGenerator
{
    /// <summary>
    /// Control Binding Model Source Generator for Blazor.
    /// </summary>
    [Generator]
    public sealed class AvaloniaControlBindingModelSourceGenerator : AbstractControlBindingModelSourceGenerator
    {
        /// <inheritdoc />
        protected override MetadataReference? CheckIfShouldAddMissingAssemblyReference(string assemblyOfInterest)
        {
            return null;
        }

        /// <inheritdoc />
        protected override IPlatformResolver GetPlatformResolver()
        {
            return new AvaloniaPlatformResolver();
        }

        /// <inheritdoc />
        protected override string GetPlatformName()
        {
            return "Avalonia";
        }
    }
}
