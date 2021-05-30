using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Vetuviem.SourceGenerator.Features.ViewBindingHelpers;
using Vetuviem.SourceGenerator.Features.ViewBindingModels;

namespace Vetuviem.SourceGenerator.GeneratorProcessors
{
    public sealed class ViewBindingHelperGeneratorProcessor : AbstractGeneratorProcessor<ViewBindingHelperClassGenerator>
    {
    }
}
