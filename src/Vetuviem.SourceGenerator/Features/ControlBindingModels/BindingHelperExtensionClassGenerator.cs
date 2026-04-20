// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Vetuviem.SourceGenerator.Features.Configuration;
using Vetuviem.SourceGenerator.Features.Core;

namespace Vetuviem.SourceGenerator.Features.ControlBindingModels
{
    /// <summary>
    /// Class generator for producing extension classes that contain helper methods for the generated Binding Model classes.
    /// </summary>
    public sealed class BindingHelperExtensionClassGenerator : IClassGenerator
    {
        /// <inheritdoc/>
        public ClassDeclarationSyntax GenerateClass(
            INamedTypeSymbol namedTypeSymbol,
            string baseUiElement,
            string? desiredCommandInterface,
            string platformName,
            string rootNamespace,
            bool makeClassesPublic,
            bool includeObsoleteItems,
            string? platformCommandType,
            bool allowExperimentalProperties,
            LoggingImplementationMode loggingImplementationMode)
        {
            var classNameIdentifier = SyntaxFactory.Identifier("ExpressionExtensions");

            var modifiers = SyntaxFactory.TokenList(
                SyntaxFactory.Token(makeClassesPublic ? SyntaxKind.PublicKeyword : SyntaxKind.InternalKeyword),
                SyntaxFactory.Token(SyntaxKind.StaticKeyword),
                SyntaxFactory.Token(SyntaxKind.PartialKeyword));

            var classDeclaration = SyntaxFactory.ClassDeclaration(classNameIdentifier)
                .WithModifiers(modifiers);

            return classDeclaration;
        }
    }
}
