using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vetuviem.SourceGenerator.Features.Core
{
    public static class NamedTypeSymbolHelpers
    {
        public static bool HasDesiredBaseType(
            string desiredBaseType,
            bool desiredBaseTypeIsInterface,
            INamedTypeSymbol namedTypeSymbol)
        {
            var baseType = namedTypeSymbol;

            while (baseType != null)
            {
                var baseTypeFullName = baseType.GetFullName();
                if (desiredBaseTypeIsInterface)
                {
                    var interfaces = baseType.Interfaces;
                    if (interfaces != null && baseType.Interfaces.Any(i => i.GetFullName().Equals(desiredBaseType, StringComparison.Ordinal)))
                    {
                        return true;
                    }
                }
                else
                {
                    if (baseTypeFullName.Equals(desiredBaseType, StringComparison.Ordinal))
                    {
                        return true;
                    }
                }

                if (baseTypeFullName.Equals("global::System.Object", StringComparison.Ordinal))
                {
                    // we can drop out 1 iteration early
                    return false;
                }

                baseType = baseType.BaseType;
            }

            return false;
        }

        public static TypeArgumentListSyntax GetTypeArgumentListSyntax(INamedTypeSymbol namedTypeSymbol)
        {
#pragma warning disable SA1129 // Do not use default value type constructor
            var sep = GetTypeArgumentSeparatedSyntaxList(namedTypeSymbol);
#pragma warning restore SA1129 // Do not use default value type constructor
            var typeArgumentList = SyntaxFactory.TypeArgumentList(sep);

            return typeArgumentList;
        }

        public static SeparatedSyntaxList<TypeSyntax> GetTypeArgumentSeparatedSyntaxList(
            INamedTypeSymbol namedTypeSymbol)
        {
            var viewForParameter = SyntaxFactory.ParseTypeName("TView");
            var viewModelParameter = SyntaxFactory.ParseTypeName("TViewModel");
            var controlParameter = SyntaxFactory.ParseTypeName(namedTypeSymbol.GetFullName());
#pragma warning disable SA1129 // Do not use default value type constructor
            var sep = new SeparatedSyntaxList<TypeSyntax>();
#pragma warning restore SA1129 // Do not use default value type constructor
            sep = sep.AddRange(new[] { viewForParameter, viewModelParameter, controlParameter });

            if (namedTypeSymbol is { IsGenericType: true })
            {
                sep = sep.AddRange(GetTypeArgumentsFromTypeParameters(namedTypeSymbol));
            }

            return sep;
        }
        /// <summary>
        /// Gets the Type Arguments based on the type symbol of a base class.
        /// </summary>
        /// <param name="baseClass">Base class to check.</param>
        /// <returns>Collection of Tye Arguments.</returns>
        public static IEnumerable<TypeSyntax> GetTypeArgumentsFromTypeParameters(INamedTypeSymbol baseClass)
        {
            foreach (var typeParameterSymbol in baseClass.TypeArguments)
            {
                if (typeParameterSymbol.Name.Equals("TViewModel", StringComparison.Ordinal))
                {
                    // quick hack for rxui already using TViewModel, will change vetuviem to use TBinding...
                    // in theory they should be the same type anyway, but not guaranteed.
                    continue;
                }

                // this deals with nullable hiding the type we're prefixing.
                var typeToCheckForGlobalPrefix =
                    typeParameterSymbol is INamedTypeSymbol namedTypeSymbol &&
                    typeParameterSymbol.Name.Equals("Nullable")
                        ? namedTypeSymbol.TypeArguments.First()
                        : typeParameterSymbol;

                string typeName = (typeToCheckForGlobalPrefix.TypeKind != TypeKind.TypeParameter && typeToCheckForGlobalPrefix.SpecialType == SpecialType.None ? "global::" : string.Empty)
                                  + typeParameterSymbol.ToDisplayString();

                yield return SyntaxFactory.ParseTypeName(typeName);
            }
        }

        public static IEnumerable<TypeParameterSyntax> GetTypeParameterSeparatedSyntaxList(INamedTypeSymbol namedTypeSymbol)
        {
            foreach (var typeParameterSymbol in namedTypeSymbol.TypeParameters)
            {
                if (typeParameterSymbol.Name.Equals("TViewModel", StringComparison.Ordinal))
                {
                    // quick hack for rxui already using TViewModel, will change vetuviem to use TBinding...
                    // in theory they should be the same type anyway, but not guaranteed.
                    continue;
                }

                yield return SyntaxFactory.TypeParameter(typeParameterSymbol.Name);
            }
        }

        public static TypeParameterListSyntax GetTypeParameterListSyntax(
            INamedTypeSymbol namedTypeSymbol,
            Func<SeparatedSyntaxList<TypeParameterSyntax>> getTypeParameterSyntaxesFunc)
        {
            var sep = getTypeParameterSyntaxesFunc();

            if (namedTypeSymbol.IsGenericType)
            {
                sep = sep.AddRange(GetTypeParameterSeparatedSyntaxList(namedTypeSymbol));
            }

            var typeParameterList = SyntaxFactory.TypeParameterList(sep);
            return typeParameterList;
        }
    }
}
