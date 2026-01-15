// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Vetuviem.SourceGenerator.Features.Core;

namespace Vetuviem.SourceGenerator.Features.WinUi
{
    /// <summary>
    /// Source Generator for Control Binding Models for the WinUI3 platform.
    /// </summary>
    [Generator]
    public sealed class WinUi3ControlBindingModelSourceGenerator : AbstractControlBindingModelSourceGenerator
    {
        /// <inheritdoc />
        protected override MetadataReference? CheckIfShouldAddMissingAssemblyReference(string assemblyOfInterest)
        {
            return null;
        }

        /// <inheritdoc />
        protected override string GetPlatformName()
        {
            return "WinUi3";
        }

        /// <inheritdoc />
        protected override IPlatformResolver GetPlatformResolver()
        {
            return new WinUi3PlatformResolver();
        }
    }
}
