using System;
using Microsoft.CodeAnalysis;
using Vetuviem.SourceGenerator;

namespace ReactiveUI.WPF.ViewToViewModelBindings
{
    [Generator]
    public sealed class WpfViewBindingHelperGenerator : AbstractViewBindingHelperGenerator
    {
        public string UiBaseType => "System.Windows.UIElement";
    }
}
