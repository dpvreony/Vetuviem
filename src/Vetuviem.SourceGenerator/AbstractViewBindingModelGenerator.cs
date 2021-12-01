using Vetuviem.SourceGenerator.Features.ControlBindingModels;

namespace Vetuviem.SourceGenerator
{
    public abstract class AbstractControlBindingModelGenerator : AbstractBaseGenerator<ControlBindingModelGeneratorProcessor>
    {
        protected override string GetNamespace() => $"ReactiveUI.{GetPlatformName()}.ViewToViewModelBindings";
    }
}
