using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.Decompiler.TypeSystem;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pharmacist.Core.BindingModels;
using Pharmacist.Core.Generation;
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
                    $"global::{fullName}.{prop.Name}");

                var propertySymbol = prop as IPropertySymbol;
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
            var bindingType = prop.IsReadOnly ? "One" : "OneOrTwo";

            var returnType = prop.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            var type = SyntaxFactory.ParseTypeName($"ReactiveUI.Core.ViewBindingModels.I{bindingType}WayBind<TViewModel, {returnType}>");

            var result = SyntaxFactory.PropertyDeclaration(type, prop.Name)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            result = result
                .WithAccessorList(
                    SyntaxFactory.AccessorList(SyntaxFactory.List(accessorList)))
                .WithLeadingTrivia(summary);

            return result;
        }

        public static SyntaxList<MemberDeclarationSyntax> GetProperties()
        {
            throw new NotImplementedException();
        }
    }
}
