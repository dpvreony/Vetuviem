// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Vetuviem.SourceGenerator.Features.Core;

namespace Vetuviem.SourceGenerator.Features.ControlBindingModels
{
    public abstract class AbstractControlBindingModelClassGenerator : IClassGenerator
    {
        public ClassDeclarationSyntax GenerateClass(
            INamedTypeSymbol namedTypeSymbol,
            string baseUiElement,
            string? desiredCommandInterface,
            string platformName)
        {
            var typeParameterList = GetTypeParameterListSyntax(namedTypeSymbol);

            var controlClassFullName = namedTypeSymbol.GetFullName();

            var modifiers = SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
            modifiers = GetClassModifiers(modifiers);

            var constraintClauses = GetTypeParameterConstraintClauseSyntaxes(controlClassFullName, namedTypeSymbol);

            var classNameIdentifier = GetClassNameIdentifier(namedTypeSymbol);

            var classDeclaration = SyntaxFactory.ClassDeclaration(classNameIdentifier);

            classDeclaration = ApplyBaseClassDeclarationSyntax(
                namedTypeSymbol,
                baseUiElement,
                controlClassFullName,
                classDeclaration,
                platformName);

            var isDerivedType = !controlClassFullName.Equals(baseUiElement, StringComparison.OrdinalIgnoreCase) && namedTypeSymbol.BaseType?.BaseType != null;

            var members = new SyntaxList<MemberDeclarationSyntax>(GetConstructorMethod(namedTypeSymbol, isDerivedType));

            members = ApplyMembers(members, namedTypeSymbol, desiredCommandInterface, isDerivedType, controlClassFullName, platformName);

            return classDeclaration
                .WithModifiers(modifiers)
                .WithTypeParameterList(typeParameterList)
                .WithConstraintClauses(constraintClauses)
                .WithLeadingTrivia(XmlSyntaxFactory.GenerateSummarySeeAlsoComment(
                    "A class that contains Viewmodel bindings for the {0} control.",
                    controlClassFullName))
                .WithMembers(members);
        }

        protected abstract SyntaxTokenList GetClassModifiers(SyntaxTokenList modifiers);

        protected abstract SyntaxList<MemberDeclarationSyntax> ApplyMembers(
            SyntaxList<MemberDeclarationSyntax> members,
            INamedTypeSymbol namedTypeSymbol,
            string? desiredCommandInterface, bool isDerivedType, string controlClassFullName, string platformName);

        protected abstract string GetClassNameIdentifier(INamedTypeSymbol namedTypeSymbol);

        private MemberDeclarationSyntax GetConstructorMethod(
            INamedTypeSymbol namedTypeSymbol,
            bool isDerivedType)
        {
            var className = GetClassNameIdentifier(namedTypeSymbol);
            var body = GetConstructorBody(isDerivedType);

            var constructorControlTypeName = GetConstructorControlTypeName(namedTypeSymbol);

            var parameters = RoslynGenerationHelpers.GetParams(new []
            {
                $"global::System.Linq.Expressions.Expression<global::System.Func<TView, {constructorControlTypeName}>> viewExpression",
            });

            var seperatedSyntaxList = new SeparatedSyntaxList<ArgumentSyntax>();

            seperatedSyntaxList = seperatedSyntaxList.Add(
                SyntaxFactory.Argument(
                    SyntaxFactory.IdentifierName(SyntaxFactory.Identifier("viewExpression"))));

            var baseInitializerArgumentList = SyntaxFactory.ArgumentList(seperatedSyntaxList);

            var initializer = SyntaxFactory.ConstructorInitializer(
                SyntaxKind.BaseConstructorInitializer,
                baseInitializerArgumentList);

            var summaryText = GetConstructorSummaryText(className);
            var summaryParameters = new (string paramName, string paramText)[]
            {
                ("viewExpression", "expression representing the control on the view to bind to.")
            };

            var declaration = SyntaxFactory.ConstructorDeclaration(className)
                .WithInitializer(initializer)
                .WithParameterList(parameters)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddBodyStatements(body.ToArray())
                .WithLeadingTrivia(XmlSyntaxFactory.GenerateSummaryComment(summaryText, summaryParameters));

            return declaration;
        }

        protected abstract string GetConstructorSummaryText(string className);

        protected abstract IReadOnlyCollection<StatementSyntax> GetConstructorBody(bool isDerivedType);

        protected abstract string GetConstructorControlTypeName(INamedTypeSymbol namedTypeSymbol);

        protected abstract ClassDeclarationSyntax ApplyBaseClassDeclarationSyntax(
            INamedTypeSymbol namedTypeSymbol,
            string baseUiElement,
            string controlClassFullName,
            ClassDeclarationSyntax classDeclaration,
            string platformName);

        protected abstract SyntaxList<TypeParameterConstraintClauseSyntax> GetTypeParameterConstraintClauseSyntaxes(
            string controlClassFullName,
            INamedTypeSymbol namedTypeSymbol);

        protected static void ApplyTypeConstraintsFromNamedTypedSymbol(
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

                var hasNotNullConstraint = typeParameterSymbol.HasNotNullConstraint;
                if (hasNotNullConstraint)
                {
                    var notNullIdentifierName = SyntaxFactory.IdentifierName("notnull");
                    var notNullTypeConstraint = SyntaxFactory.TypeConstraint(notNullIdentifierName);

                    typeParameterConstraintSyntaxList.Add(notNullTypeConstraint);
                }

#if TODO
                var constraintNullableAnnotations = typeParameterSymbol.ConstraintNullableAnnotations;
                var hasConstructorConstraint = typeParameterSymbol.HasConstructorConstraint;
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

        private TypeParameterListSyntax GetTypeParameterListSyntax(INamedTypeSymbol namedTypeSymbol)
        {
            var sep = GetTypeParameterSyntaxes();

            if (namedTypeSymbol.IsGenericType)
            {
                sep = sep.AddRange(GetTypeParameterSeparatedSyntaxList(namedTypeSymbol));
            }

            var typeParameterList = SyntaxFactory.TypeParameterList(sep);
            return typeParameterList;
        }

        protected abstract SeparatedSyntaxList<TypeParameterSyntax> GetTypeParameterSyntaxes();

        private static IEnumerable<TypeParameterSyntax> GetTypeParameterSeparatedSyntaxList(INamedTypeSymbol namedTypeSymbol)
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

        protected static IEnumerable<TypeSyntax> GetTypeArgumentsFromTypeParameters(INamedTypeSymbol baseClass)
        {
            foreach (var typeParameterSymbol in baseClass.TypeArguments)
            {
                if (typeParameterSymbol.Name.Equals("TViewModel", StringComparison.Ordinal))
                {
                    // quick hack for rxui already using TViewModel, will change vetuviem to use TBinding...
                    // in theory they should be the same type anyway, but not guaranteed.
                    continue;
                }

                yield return SyntaxFactory.ParseTypeName(typeParameterSymbol.ToDisplayString());
            }
        }
    }
}
