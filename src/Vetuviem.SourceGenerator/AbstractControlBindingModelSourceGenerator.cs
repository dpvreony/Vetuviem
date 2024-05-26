// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Vetuviem.SourceGenerator.Features.ControlBindingModels;

namespace Vetuviem.SourceGenerator
{
    /// <summary>
    /// Abstraction for a Code Generator for a Control Binding Model.
    /// </summary>
    public abstract class AbstractControlBindingModelSourceGenerator : AbstractBaseSourceGenerator<ControlBindingModelGeneratorProcessor>
    {
        /// <param name="rootNamespace"></param>
        /// <inheritdoc />
        protected override string GetNamespace(string? rootNamespace)
        {
            if (string.IsNullOrWhiteSpace(rootNamespace))
            {
                rootNamespace = "VetuviemGenerated";
            }

            return $"{rootNamespace}.{GetPlatformName()}.ViewToViewModelBindings";
        }
    }
}
