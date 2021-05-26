using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Vetuviem.SourceGenerator.Features.Core;
using ISymbol = Microsoft.CodeAnalysis.ISymbol;
using SymbolKind = Microsoft.CodeAnalysis.SymbolKind;

namespace Vetuviem.SourceGenerator.Features.ViewBindingModels
{
    /// <summary>
    /// Generates a ViewBindingModel for the discovered types.
    /// </summary>
    public static class ViewBindingModelPropertyGenerator
    {
        public static SyntaxList<MemberDeclarationSyntax> GetProperties(
            INamedTypeSymbol namedTypeSymbol)
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

                var propSyntax = GetPropertyDeclaration(propertySymbol, accessorList, summary);

                nodes.Add(propSyntax);
            }

            return new SyntaxList<MemberDeclarationSyntax>(nodes);
        }

        private static PropertyDeclarationSyntax GetPropertyDeclaration(
            IPropertySymbol prop,
            AccessorDeclarationSyntax[] accessorList,
            IEnumerable<SyntaxTrivia> summary)
        {
            TypeSyntax type = GetBindingTypeSyntax(prop);

            var result = SyntaxFactory.PropertyDeclaration(type, prop.Name)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            result = result
                .WithAccessorList(
                    SyntaxFactory.AccessorList(SyntaxFactory.List(accessorList)))
                .WithLeadingTrivia(summary);

            return result;
        }

        private static TypeSyntax GetBindingTypeSyntax(IPropertySymbol prop)
        {
            string bindingName = GetBindingInterfaceName(prop);

            var returnType = prop.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            var type = SyntaxFactory.ParseTypeName($"global::Vetuviem.Core.{bindingName}<TViewModel, {returnType}>");
            return type;
        }

        private static string GetBindingInterfaceName(IPropertySymbol prop)
        {
            var propType = prop.Type;
            var isCommand = propType.AllInterfaces.Any(interfaceName => interfaceName.GetFullName().Equals("global::System.Windows.Input.ICommand"));
            if (isCommand)
            {
                return "ICommandBinding";
            }

            var bindingType = prop.IsReadOnly ? "One" : "OneOrTwo";

            return $"I{bindingType}WayBind";
        }
    }
}
