using Vetuviem.SourceGenerator.Features.ViewBindingModels;

namespace Vetuviem.SourceGenerator
{
    public abstract class AbstractViewBindingModelGenerator : AbstractBaseGenerator<ViewBindingModelGeneratorProcessor>
    {
        protected override string GetNamespace() => $"ReactiveUI.{GetPlatformName()}.ViewToViewModelBindings";
    }
}
