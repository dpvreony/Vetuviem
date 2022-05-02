// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using Vetuviem.SourceGenerator.Features.Core;

namespace Vetuviem.SourceGenerator.Features.ControlBindingModels
{
    /// <summary>
    /// Process Manager for Generating Code for Control Binding Models.
    /// </summary>
    public sealed class ControlBindingModelGeneratorProcessor : AbstractGeneratorProcessor
    {
        /// <inheritdoc/>
        protected override Func<IClassGenerator>[] GetClassGenerators()
        {
            return new Func<IClassGenerator>[]
            {
                () => new GenericControlBindingModelClassGenerator(),
                () => new ControlBoundControlBindingModelClassGenerator(),
            };
        }
    }
}
