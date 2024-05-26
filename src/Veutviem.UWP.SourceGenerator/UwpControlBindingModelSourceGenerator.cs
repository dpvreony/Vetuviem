// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Vetuviem.SourceGenerator;
using Vetuviem.SourceGenerator.Features.Core;

namespace Veutviem.UWP.SourceGenerator
{
    /// <summary>
    /// Source Generator for UWP View Binding Models.
    /// </summary>
    [Generator]
    public sealed class UwpControlBindingModelSourceGenerator : AbstractControlBindingModelSourceGenerator
    {
        /// <inheritdoc />
        protected override MetadataReference CheckIfShouldAddMissingAssemblyReference(string assemblyOfInterest)
        {
            return MetadataReference.CreateFromFile(assemblyOfInterest);
        }

        /// <inheritdoc />
        protected override IPlatformResolver GetPlatformResolver()
        {
            return new UwpPlatformResolver();
        }

        /// <inheritdoc/>
        protected override string GetPlatformName()
        {
            return "UWP";
        }
    }
}
