using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vetuviem.SourceGenerator.Features.Core
{
    public interface IClassGenerator
    {
        ClassDeclarationSyntax GenerateClass(
            INamedTypeSymbol namedTypeSymbol,
            string baseUiElement,
            string desiredCommandInterface,
            string platformName);
    }
}
