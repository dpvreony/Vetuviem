using System;
using Vetuviem.SourceGenerator.Features.Core;

namespace Vetuviem.SourceGenerator.Features.ViewBindingModels
{
    public sealed class ViewBindingModelGeneratorProcessor : AbstractGeneratorProcessor
    {
        protected override Func<IClassGenerator>[] GetClassGenerators()
        {
            return new Func<IClassGenerator>[]
            {
                () => new GenericViewBindingModelClassGenerator(),
                () => new ControlBoundViewBindingModelClassGenerator(),
            };
        }
    }
}
