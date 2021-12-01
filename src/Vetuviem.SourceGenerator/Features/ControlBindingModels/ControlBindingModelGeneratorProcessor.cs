using System;
using Vetuviem.SourceGenerator.Features.Core;

namespace Vetuviem.SourceGenerator.Features.ControlBindingModels
{
    public sealed class ControlBindingModelGeneratorProcessor : AbstractGeneratorProcessor
    {
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
