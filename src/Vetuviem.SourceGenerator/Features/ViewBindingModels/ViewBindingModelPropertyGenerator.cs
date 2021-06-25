using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Vetuviem.SourceGenerator.Features.Core;

namespace Vetuviem.SourceGenerator.Features.ViewBindingModels
{
    /// <summary>
    /// Generates a ViewBindingModel for the discovered types.
    /// </summary>
    public static class ViewBindingModelPropertyGenerator
    {
        /// <summary>
        /// Gets the properties to be generated.
        /// </summary>
        /// <param name="namedTypeSymbol">The type to check the properties on.</param>
        /// <param name="desiredCommandInterface">The fully qualified typename for the Command interface used by the UI platform, if it uses one.</param>
        /// <returns>List of property declarations.</returns>
        public static SyntaxList<MemberDeclarationSyntax> GetProperties(
            INamedTypeSymbol namedTypeSymbol,
            string desiredCommandInterface,
            bool isDerivedType,
            string controlClassFullName)
        {
            var properties = namedTypeSymbol
                .GetMembers()
                .Where(x => x.Kind == SymbolKind.Property)
                .ToArray();

            var nodes = new List<MemberDeclarationSyntax>(properties.Length);

            foreach (var prop in properties)
            {
                var propertySymbol = prop as IPropertySymbol;

                if (propertySymbol == null
                    || propertySymbol.IsIndexer
                    || propertySymbol.IsOverride
                    || propertySymbol.DeclaredAccessibility != Accessibility.Public
                    || propertySymbol.ExplicitInterfaceImplementations.Any())
                {
                    continue;
                }

                // windows forms has an issue where some properties are provided as "new" instances instead of overridden
                // we're getting build warnings for these.
                if (ReplacesBaseProperty(propertySymbol, namedTypeSymbol))
                {
                    // for now we skip, but we may adjust our model moving forward to make them "new".
                    continue;
                }

                var accessorList = new[]
                {
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                };

                var fullName = namedTypeSymbol.GetFullName();

                var summary = XmlSyntaxFactory.GenerateSummarySeeAlsoComment(
                    "Gets or sets the binding logic for {0}",
                    $"{fullName}.{prop.Name}");

                var propSyntax = GetPropertyDeclaration(
                    propertySymbol,
                    accessorList,
                    summary,
                    desiredCommandInterface);

                nodes.Add(propSyntax);
            }

            return new SyntaxList<MemberDeclarationSyntax>(nodes);
        }

        private static bool ReplacesBaseProperty(
            IPropertySymbol propertySymbol,
            INamedTypeSymbol namedTypeSymbol)
        {
            var wantedName = propertySymbol.Name;
            var baseType = namedTypeSymbol.BaseType;
            while (baseType != null)
            {
                var nameMatches = baseType.GetMembers()
                    .Where(x => x.Kind == SymbolKind.Property && x.Name.Equals(wantedName, StringComparison.Ordinal))
                    .Cast<IPropertySymbol>()
                    .ToImmutableArray();

                foreach (var nameMatch in nameMatches)
                {
                    if (SymbolEqualityComparer.Default.Equals(nameMatch.Type, propertySymbol.Type))
                    {
                        return true;
                    }
                }

                baseType = baseType.BaseType;
            }

            return false;
        }

        private static PropertyDeclarationSyntax GetPropertyDeclaration(
            IPropertySymbol prop,
            AccessorDeclarationSyntax[] accessorList,
            IEnumerable<SyntaxTrivia> summary,
            string desiredCommandInterface)
        {
            TypeSyntax type = GetBindingTypeSyntax(prop, desiredCommandInterface);

            var result = SyntaxFactory.PropertyDeclaration(
                    type,
                    prop.Name)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .WithAccessorList(
                    SyntaxFactory.AccessorList(SyntaxFactory.List(accessorList)))
                .WithLeadingTrivia(summary);

            return result;
        }

        private static TypeSyntax GetBindingTypeSyntax(
            IPropertySymbol prop,
            string desiredCommandInterface)
        {
            string bindingName = GetBindingInterfaceName(
                prop,
                desiredCommandInterface);

            var returnType = prop.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            var type = SyntaxFactory.ParseTypeName($"global::Vetuviem.Core.{bindingName}<TViewModel, {returnType}>");
            return type;
        }

        private static string GetBindingInterfaceName(
            IPropertySymbol prop,
            string desiredCommandInterface)
        {
            if (!string.IsNullOrWhiteSpace(desiredCommandInterface))
            {
                var propType = prop.Type;
                var isCommand = propType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Equals(desiredCommandInterface)
                    || propType.AllInterfaces.Any(interfaceName => interfaceName.GetFullName().Equals(desiredCommandInterface));
                if (isCommand)
                {
                    return "ICommandBinding";
                }
            }

            var bindingType = prop.IsReadOnly ? "One" : "OneOrTwo";

            return $"I{bindingType}WayBind";
        }
    }
}
