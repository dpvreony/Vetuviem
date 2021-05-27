using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Vetuviem.SourceGenerator.Features.Core
{
    public static class NamedTypeSymbolExtensions
    {
        public static string GetFullName(this INamedTypeSymbol namedTypeSymbol)
        {
            return namedTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        }
    }
}
