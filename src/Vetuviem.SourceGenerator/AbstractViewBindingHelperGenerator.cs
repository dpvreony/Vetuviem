using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vetuviem.SourceGenerator.GeneratorProcessors;

namespace Vetuviem.SourceGenerator
{
    public abstract class AbstractViewBindingHelperGenerator : AbstractBaseGenerator<ViewBindingHelperGeneratorProcessor>
    {
        protected override string GetNamespace() => "ReactiveUI.WPF.ViewToViewModelBindingHelpers";
    }
}
