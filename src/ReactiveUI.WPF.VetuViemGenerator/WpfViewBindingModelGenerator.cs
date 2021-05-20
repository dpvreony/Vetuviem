using Microsoft.CodeAnalysis;
using Vetuviem.SourceGenerator;

namespace ReactiveUI.WPF.ViewToViewModelBindings
{
    [Generator]
    public sealed class WpfViewBindingModelGenerator : AbstractViewBindingModelGenerator
    {
        protected override string GetNamespace() => "ReactiveUI.WPF.ViewToViewModelBinding";
    }
}
