using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Vetuviem.SourceGenerator.Features.Core;

namespace Vetuviem.SourceGenerator.Features.ViewBindingHelpers
{
    public class ViewBindingHelperClassGenerator : IClassGenerator
    {
        public ClassDeclarationSyntax GenerateClass(INamedTypeSymbol namedTypeSymbol, string baseUiElement,
            string desiredCommandInterface, string platformName)
        {
            return null;
        }
    }
}
