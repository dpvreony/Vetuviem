// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
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

            var defaultFactoryMethodDeclaration = GetDefaultFactoryMethodDeclaration(namedTypeSymbol);

            var members = new SyntaxList<MemberDeclarationSyntax>(defaultFactoryMethodDeclaration);

            var classDeclaration = SyntaxFactory.ClassDeclaration(classNameIdentifier)
                .WithModifiers(modifiers)
                .WithMembers(members)
                .WithLeadingTrivia(XmlSyntaxFactory.GenerateSummaryComment($"Extension Methods for <see cref=\"global::System.Linq.Expressions.Expression{{global::System.Func{{TView, {namedTypeSymbol.GetFullName()}}}}}\"/>."));

            return classDeclaration;
        }

        private MethodDeclarationSyntax GetDefaultFactoryMethodDeclaration(INamedTypeSymbol namedTypeSymbol)
        {
            var returnType = GetReturnType(namedTypeSymbol);

            // TODO: add other arguments as needed to determine which properties to set on the binding model instance.
            // TODO: add the method body that creates and returns an instance of the binding model, using the expression argument to determine which properties to set on the binding model instance.
            var parameters = RoslynGenerationHelpers.GetParams(
            [
                $"this global::System.Linq.Expressions.Expression<global::System.Func<TView, {namedTypeSymbol.GetFullName()}>> viewExpression",
            ]);

            StatementSyntax[] methodBody = [
                SyntaxFactory.ParseStatement($"var bindingModel = new {returnType}(viewExpression);"),
                SyntaxFactory.ParseStatement("return bindingModel;")
            ];

            var controlClassFullName = namedTypeSymbol.GetFullName();
            var constraintClauses = GetTypeParameterConstraintClauseSyntaxes(controlClassFullName, namedTypeSymbol);

            var typeParameterList = NamedTypeSymbolHelpers.GetTypeParameterListSyntax(
                namedTypeSymbol,
                () => this.GetTypeParameterSyntaxes(namedTypeSymbol));

            var summaryParameters = new (string paramName, string paramText)[]
            {
                ("viewExpression", "expression representing the control on the view to bind to.")
            };

            var defaultFactoryMethodDeclaration = SyntaxFactory.MethodDeclaration(
                    returnType,
                    "GetDefaultBindingModel")
                .WithTypeParameterList(typeParameterList)
                .WithConstraintClauses(constraintClauses)
                .AddModifiers(
                    SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                    SyntaxFactory.Token(SyntaxKind.StaticKeyword))
                .WithParameterList(parameters)
                .AddBodyStatements(methodBody)
                .WithLeadingTrivia(XmlSyntaxFactory.GenerateSummaryComment(
                    $"Creates a binding model for {controlClassFullName}.",
                    summaryParameters,
                    $"Instance of a {controlClassFullName} Binding Model."));
            return defaultFactoryMethodDeclaration;
        }

        private SyntaxList<TypeParameterConstraintClauseSyntax> GetTypeParameterConstraintClauseSyntaxes(
            string controlClassFullName, INamedTypeSymbol namedTypeSymbol)
        {
#pragma warning disable SA1129 // Do not use default value type constructor
            var viewConstraints = new SeparatedSyntaxList<TypeParameterConstraintSyntax>();
#pragma warning restore SA1129 // Do not use default value type constructor
            var viewForConstraint =
                SyntaxFactory.TypeConstraint(SyntaxFactory.ParseTypeName("global::ReactiveUI.IViewFor<TViewModel>"));

            viewConstraints = viewConstraints
                .Add(SyntaxFactory.ClassOrStructConstraint(SyntaxKind.ClassConstraint))
                .Add(viewForConstraint);
            var viewConstraintClause = SyntaxFactory.TypeParameterConstraintClause(
                SyntaxFactory.IdentifierName("TView"),
                viewConstraints);

            var reactiveObjectInterfaceConstraint =
                SyntaxFactory.TypeConstraint(SyntaxFactory.ParseTypeName("global::ReactiveUI.IReactiveObject"));
#pragma warning disable SA1129 // Do not use default value type constructor
            var viewModelConstraints = new SeparatedSyntaxList<TypeParameterConstraintSyntax>();
#pragma warning restore SA1129 // Do not use default value type constructor
            viewModelConstraints =
                viewModelConstraints
                    .Add(SyntaxFactory.ClassOrStructConstraint(SyntaxKind.ClassConstraint))
                    .Add(reactiveObjectInterfaceConstraint);
            var viewModelConstraintClause = SyntaxFactory.TypeParameterConstraintClause(
                SyntaxFactory.IdentifierName("TViewModel"),
                viewModelConstraints);

            var typeParameterConstraintClauseSyntaxList =
                new List<TypeParameterConstraintClauseSyntax>
                {
                    viewConstraintClause, viewModelConstraintClause
                };

            ApplyTypeConstraintsFromNamedTypedSymbol(
                namedTypeSymbol,
                typeParameterConstraintClauseSyntaxList);

            var constraintClauses =
                new SyntaxList<TypeParameterConstraintClauseSyntax>(typeParameterConstraintClauseSyntaxList);
            return constraintClauses;
        }

        /// <summary>
        /// Adds type constraints to the collection based on the named type.
        /// </summary>
        /// <param name="namedTypeSymbol">Named type to process.</param>
        /// <param name="typeParameterConstraintClauseSyntaxList">Collection of type parameter constraints to add to.</param>
        private static void ApplyTypeConstraintsFromNamedTypedSymbol(
            INamedTypeSymbol namedTypeSymbol,
            IList<TypeParameterConstraintClauseSyntax> typeParameterConstraintClauseSyntaxList)
        {
            if (!namedTypeSymbol.IsGenericType)
            {
                return;
            }

            foreach (var typeParameterSymbol in namedTypeSymbol.TypeParameters)
            {
                if (typeParameterSymbol.Name.Equals("TViewModel", StringComparison.Ordinal))
                {
                    // quick hack for rxui already using TViewModel, will change vetuviem to use TBinding...
                    // in theory they should be the same type anyway, but not guaranteed.
                    continue;
                }

                var typeParameterConstraintSyntaxList = new List<TypeParameterConstraintSyntax>();

                var hasReferenceTypeConstraint = typeParameterSymbol.HasReferenceTypeConstraint;
                if (hasReferenceTypeConstraint)
                {
                    typeParameterConstraintSyntaxList.Add(SyntaxFactory.ClassOrStructConstraint(SyntaxKind.ClassConstraint));
                }

                else if (typeParameterSymbol.HasValueTypeConstraint)
                {
                    typeParameterConstraintSyntaxList.Add(SyntaxFactory.ClassOrStructConstraint(SyntaxKind.StructConstraint));
                }

                else
                {
                    var hasNotNullConstraint = typeParameterSymbol.HasNotNullConstraint;
                    if (hasNotNullConstraint || AnyBaseHasNotNullConstraint(namedTypeSymbol, typeParameterSymbol))
                    {
                        var notNullIdentifierName = SyntaxFactory.IdentifierName("notnull");
                        var notNullTypeConstraint = SyntaxFactory.TypeConstraint(notNullIdentifierName);

                        typeParameterConstraintSyntaxList.Add(notNullTypeConstraint);
                    }
                }

#if TODO
                var constraintNullableAnnotations = typeParameterSymbol.ConstraintNullableAnnotations;
                var hasUnmanagedTypeConstraint = typeParameterSymbol.HasUnmanagedTypeConstraint;
                var hasValueTypeConstraint = typeParameterSymbol.HasValueTypeConstraint;
                var referenceTypeConstraintNullableAnnotation = typeParameterSymbol.ReferenceTypeConstraintNullableAnnotation;
#endif

                var constraintTypes = typeParameterSymbol.ConstraintTypes;
                foreach (var constraintType in constraintTypes)
                {
                    var constraintToAdd = SyntaxFactory.TypeConstraint(
                        SyntaxFactory.ParseTypeName(constraintType.ToDisplayString(
                            SymbolDisplayFormat.FullyQualifiedFormat)));
                    typeParameterConstraintSyntaxList.Add(constraintToAdd);
                }

                // new() constraint must be last, so we add it after processing the others.
                var hasConstructorConstraint = typeParameterSymbol.HasConstructorConstraint;
                if (hasConstructorConstraint)
                {
                    var constructorConstraint = SyntaxFactory.ConstructorConstraint();
                    typeParameterConstraintSyntaxList.Add(constructorConstraint);
                }

                if (typeParameterConstraintSyntaxList.Count < 1)
                {
                    continue;
                }

                var newTypeParameterContraint = SyntaxFactory.SeparatedList(typeParameterConstraintSyntaxList);

                var newTypeParameterConstraintClause = SyntaxFactory.TypeParameterConstraintClause(
                    SyntaxFactory.IdentifierName(typeParameterSymbol.Name),
                    newTypeParameterContraint);

                typeParameterConstraintClauseSyntaxList.Add(newTypeParameterConstraintClause);
            }
        }

        private static bool AnyBaseHasNotNullConstraint(INamedTypeSymbol derivedTypeSymbol, ITypeParameterSymbol typeParameterSymbol)
        {
            // Find the index of the type parameter in the derived class
            int paramIndex = derivedTypeSymbol.TypeParameters.IndexOf(typeParameterSymbol);
            if (paramIndex < 0)
                return false;

            INamedTypeSymbol? current = derivedTypeSymbol.BaseType;
            while (current != null && current.TypeParameters.Length > paramIndex)
            {
                var baseTypeParam = current.TypeParameters[paramIndex];
                if (baseTypeParam.HasNotNullConstraint)
                    return true;

                current = current.BaseType;
            }
            return false;
        }

        private static TypeSyntax GetReturnType(INamedTypeSymbol namedTypeSymbol)
        {
            if (namedTypeSymbol == null)
            {
                throw new ArgumentNullException(nameof(namedTypeSymbol));
            }

            return SyntaxFactory.ParseTypeName($"{namedTypeSymbol.Name}ControlBindingModel<TView, TViewModel>");
        }

        private SeparatedSyntaxList<TypeParameterSyntax> GetTypeParameterSyntaxes(INamedTypeSymbol namedTypeSymbol)
        {
            var viewForParameter = SyntaxFactory.TypeParameter("TView");
            var viewModelParameter = SyntaxFactory.TypeParameter("TViewModel");

#pragma warning disable SA1129 // Do not use default value type constructor
            var sep = new SeparatedSyntaxList<TypeParameterSyntax>();
#pragma warning restore SA1129 // Do not use default value type constructor
            sep = sep.AddRange([viewForParameter, viewModelParameter]);

            return sep;
        }
    }
}
