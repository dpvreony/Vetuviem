﻿using Vetuviem.SourceGenerator.Features.ViewBindingModels;
using Vetuviem.SourceGenerator.GeneratorProcessors;

namespace Vetuviem.SourceGenerator
{
    public abstract class AbstractViewBindingModelGenerator : AbstractBaseGenerator<ViewBindingModelGeneratorProcessor, AbstractViewBindingModelClassGenerator>
    {
        protected override string GetNamespace() => $"ReactiveUI.{GetPlatformName()}.ViewToViewModelBindings";
    }
}
