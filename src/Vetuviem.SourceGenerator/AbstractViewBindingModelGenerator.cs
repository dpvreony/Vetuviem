// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Vetuviem.SourceGenerator.Features.ControlBindingModels;

namespace Vetuviem.SourceGenerator
{
    public abstract class AbstractControlBindingModelGenerator : AbstractBaseGenerator<ControlBindingModelGeneratorProcessor>
    {
        protected override string GetNamespace() => $"ReactiveUI.{GetPlatformName()}.ViewToViewModelBindings";
    }
}
