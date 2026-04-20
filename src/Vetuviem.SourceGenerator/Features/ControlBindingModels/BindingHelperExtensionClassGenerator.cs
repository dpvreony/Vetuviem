// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reflection.Metadata;
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

            var returnType = GetReturnType(namedTypeSymbol);

            // TODO: add the expression argument
            // TODO: add other arguments as needed to determine which properties to set on the binding model instance.
            // TODO: add the method body that creates and returns an instance of the binding model, using the expression argument to determine which properties to set on the binding model instance.
            var parameters = RoslynGenerationHelpers.GetParams(new[]
            {
                $"this Expression<Func<TView, {namedTypeSymbol.GetFullName()}>> controlExpression",
            });

            var methodBody = SyntaxFactory.Block(
                SyntaxFactory.ParseStatement($"var bindingModel = new {returnType}();"),
                SyntaxFactory.ParseStatement("// TODO: use the controlExpression to determine which properties to set on the binding model instance."),
                SyntaxFactory.ParseStatement("return bindingModel;"));

            var defaultFactoryMethodDeclaration = SyntaxFactory.MethodDeclaration(
                returnType,
                "GetDefaultBindingModel")
                .AddModifiers(
                    SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                    SyntaxFactory.Token(SyntaxKind.StaticKeyword))
                .WithParameterList(parameters)
                .AddBodyStatements(methodBody)
                .WithLeadingTrivia(XmlSyntaxFactory.InheritdocSyntax);


            var members = new SyntaxList<MemberDeclarationSyntax>(defaultFactoryMethodDeclaration);

            var classDeclaration = SyntaxFactory.ClassDeclaration(classNameIdentifier)
                .WithModifiers(modifiers)
                .WithMembers(members);

            return classDeclaration;
        }

        /// <inheritdoc />
        private static TypeSyntax GetReturnType(INamedTypeSymbol namedTypeSymbol)
        {
            if (namedTypeSymbol == null)
            {
                throw new ArgumentNullException(nameof(namedTypeSymbol));
            }

            return SyntaxFactory.ParseTypeName($"{namedTypeSymbol.Name}ControlBindingModel");
        }
    }
}
