// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Vetuviem.SourceGenerator.Features.Core;

namespace Vetuviem.SourceGenerator.Features.ControlBindingModels
{
    /// <summary>
    /// Generates a Control Binding Model for the discovered types.
    /// </summary>
    public static class ControlBindingModelPropertyGenerator
    {
        /// <summary>
        /// Gets the properties to be generated.
        /// </summary>
        /// <param name="namedTypeSymbol">The type to check the properties on.</param>
        /// <param name="desiredCommandInterface">The fully qualified typename for the Command interface used by the UI platform, if it uses one.</param>
        /// <param name="makeClassesPublic">A flag indicating whether to expose the generated binding classes as public rather than internal. Set this to true if you're created a reusable library file.</param>
        /// <param name="includeObsoleteItems">Whether to include obsolete items in the generated code.</param>
        /// <returns>List of property declarations.</returns>
        public static SyntaxList<MemberDeclarationSyntax> GetProperties(
            INamedTypeSymbol namedTypeSymbol,
            string? desiredCommandInterface,
            bool makeClassesPublic,
            bool includeObsoleteItems,
            string? platformCommandType)
        {
            if (namedTypeSymbol == null)
            {
                throw new ArgumentNullException(nameof(namedTypeSymbol));
            }

            var properties = namedTypeSymbol
                .GetMembers()
                .Where(x => x.Kind == SymbolKind.Property)
                .ToArray();

            var fullName = namedTypeSymbol.GetFullName();

            var nodes = new List<MemberDeclarationSyntax>(properties.Length);

            if (ShouldGenerateCommandBindingProperty(namedTypeSymbol, desiredCommandInterface, platformCommandType))
            {
                var bindCommandPropertyDeclaration = GetBindCommandPropertyDeclaration(
                    makeClassesPublic,
                    fullName,
                    platformCommandType!);
                nodes.Add(bindCommandPropertyDeclaration);
            }

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

                // check for obsolete attribute
                var attributes = propertySymbol.GetAttributes();
                if (!includeObsoleteItems && attributes.Any(a => a.AttributeClass?.GetFullName().Equals(
                        "global::System.ObsoleteAttribute",
                        StringComparison.Ordinal) == true))
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

                var accessorList = GetAccessorDeclarationSyntaxes();

                var summary = XmlSyntaxFactory.GenerateSummarySeeAlsoComment(
                    "Gets or sets the binding logic for {0}",
                    $"{fullName}.{prop.Name}");

                var propSyntax = GetPropertyDeclaration(
                    propertySymbol,
                    accessorList,
                    summary,
                    desiredCommandInterface,
                    makeClassesPublic);

                nodes.Add(propSyntax);
            }

            return new SyntaxList<MemberDeclarationSyntax>(nodes);
        }

        private static bool ShouldGenerateCommandBindingProperty(INamedTypeSymbol namedTypeSymbol, string? desiredCommandInterface, string? platformCommandType)
        {
            return !string.IsNullOrWhiteSpace(desiredCommandInterface)
                   && !string.IsNullOrWhiteSpace(platformCommandType)
                   && namedTypeSymbol.Interfaces.Any(interfaceName => interfaceName.GetFullName().Equals(desiredCommandInterface, StringComparison.Ordinal))
                   // we don't want to generate the property if the base class already has it
                   // this happens if someone incorrectly applies the interface on a subclass as well as the base class
                   && (namedTypeSymbol.BaseType == null || namedTypeSymbol.BaseType.Interfaces.All(interfaceName => !interfaceName.GetFullName().Equals(desiredCommandInterface, StringComparison.Ordinal)));
        }

        private static AccessorDeclarationSyntax[] GetAccessorDeclarationSyntaxes()
        {
            var accessorList = new[]
            {
                SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                SyntaxFactory.AccessorDeclaration(SyntaxKind.InitAccessorDeclaration)
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
            };
            return accessorList;
        }

        private static MemberDeclarationSyntax GetBindCommandPropertyDeclaration(
            bool makeClassesPublic,
            string fullName,
            string platformCommandType)
        {
            var accessorList = GetAccessorDeclarationSyntaxes();

            var summary = XmlSyntaxFactory.GenerateSummarySeeAlsoComment(
                "Gets or sets the command binding logic for {0}",
                fullName);

            return GetBindCommandPropertyDeclaration(
                accessorList,
                summary,
                makeClassesPublic,
                platformCommandType);
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

        private static PropertyDeclarationSyntax GetBindCommandPropertyDeclaration(
            AccessorDeclarationSyntax[] accessorList,
            IEnumerable<SyntaxTrivia> summary,
            bool makeClassesPublic,
            string platformCommandType)
        {
            TypeSyntax type = GetCommandBindingTypeSyntax(platformCommandType);

            var result = SyntaxFactory.PropertyDeclaration(
                    type,
                    "BindCommand")
                .AddModifiers(SyntaxFactory.Token(makeClassesPublic ? SyntaxKind.PublicKeyword : SyntaxKind.InternalKeyword))
                .WithAccessorList(
                    SyntaxFactory.AccessorList(SyntaxFactory.List(accessorList)))
                .WithLeadingTrivia(summary);

            return result;
        }

        private static PropertyDeclarationSyntax GetPropertyDeclaration(
            IPropertySymbol prop,
            AccessorDeclarationSyntax[] accessorList,
            IEnumerable<SyntaxTrivia> summary,
            string? desiredCommandInterface,
            bool makeClassesPublic)
        {
            TypeSyntax type = GetBindingTypeSyntax(prop, desiredCommandInterface);

            var result = SyntaxFactory.PropertyDeclaration(
                    type,
                    prop.Name)
                .AddModifiers(SyntaxFactory.Token(makeClassesPublic ? SyntaxKind.PublicKeyword : SyntaxKind.InternalKeyword))
                .WithAccessorList(
                    SyntaxFactory.AccessorList(SyntaxFactory.List(accessorList)))
                .WithLeadingTrivia(summary);

            return result;
        }

        private static TypeSyntax GetCommandBindingTypeSyntax(string platformCommandType)
        {
            var type = SyntaxFactory.ParseTypeName($"global::Vetuviem.Core.ICommandBinding<TViewModel>?");
            return type;
        }

        private static TypeSyntax GetBindingTypeSyntax(
            IPropertySymbol prop,
            string? desiredCommandInterface)
        {
            string bindingName = GetBindingInterfaceName(
                prop,
                desiredCommandInterface);

            var returnType = prop.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            var type = SyntaxFactory.ParseTypeName($"global::Vetuviem.Core.{bindingName}<TViewModel, {returnType}>?");
            return type;
        }

        private static string GetBindingInterfaceName(
            IPropertySymbol prop,
            string? desiredCommandInterface)
        {
            var bindingType = prop.IsReadOnly ? "One" : "OneOrTwo";

            return $"I{bindingType}WayBind";
        }
    }
}
