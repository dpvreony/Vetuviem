using Vetuviem.SourceGenerator.GeneratorProcessors;

namespace Vetuviem.SourceGenerator
{
    public abstract class AbstractViewBindingHelperGenerator : AbstractBaseGenerator<ViewBindingHelperGeneratorProcessor>
    {
        protected override string GetNamespace() => $"ReactiveUI.{GetPlatformName()}.ViewToViewModelBindingHelpers";
    }
}
