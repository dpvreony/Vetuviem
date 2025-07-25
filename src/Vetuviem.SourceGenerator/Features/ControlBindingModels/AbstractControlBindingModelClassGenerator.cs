﻿// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
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
    /// <summary>
    /// Abstraction for generating a Control Binding Model class.
    /// </summary>
    public abstract class AbstractControlBindingModelClassGenerator : IClassGenerator
    {
        /// <inheritdoc />
        public ClassDeclarationSyntax GenerateClass(
            INamedTypeSymbol namedTypeSymbol,
            string baseUiElement,
            string? desiredCommandInterface,
            string platformName,
            string rootNamespace,
            bool makeClassesPublic,
            bool includeObsoleteItems,
            string? platformCommandType)
        {
            var typeParameterList = GetTypeParameterListSyntax(namedTypeSymbol);

            var controlClassFullName = namedTypeSymbol.GetFullName();

            var modifiers = SyntaxFactory.TokenList(SyntaxFactory.Token(makeClassesPublic ? SyntaxKind.PublicKeyword : SyntaxKind.InternalKeyword));
            modifiers = GetClassModifiers(modifiers);

            var constraintClauses = GetTypeParameterConstraintClauseSyntaxes(controlClassFullName, namedTypeSymbol);

            var classNameIdentifier = GetClassNameIdentifier(namedTypeSymbol);

            var classDeclaration = SyntaxFactory.ClassDeclaration(classNameIdentifier);

            classDeclaration = ApplyBaseClassDeclarationSyntax(
                namedTypeSymbol,
                baseUiElement,
                controlClassFullName,
                classDeclaration,
                platformName,
                rootNamespace);

            var isDerivedType = !controlClassFullName.Equals(baseUiElement, StringComparison.OrdinalIgnoreCase) && namedTypeSymbol.BaseType?.BaseType != null;

            var members = new SyntaxList<MemberDeclarationSyntax>(GetConstructorMethod(namedTypeSymbol, isDerivedType, makeClassesPublic, typeParameterList));

            members = ApplyMembers(
                members,
                namedTypeSymbol,
                desiredCommandInterface,
                isDerivedType,
                controlClassFullName,
                platformName,
                makeClassesPublic,
                includeObsoleteItems,
                platformCommandType);

            return classDeclaration
                .WithModifiers(modifiers)
                .WithTypeParameterList(typeParameterList)
                .WithConstraintClauses(constraintClauses)
                .WithLeadingTrivia(XmlSyntaxFactory.GenerateSummarySeeAlsoComment(
                    "A class that contains Viewmodel bindings for the {0} control.",
                    controlClassFullName))
                .WithMembers(members);
        }

        /// <summary>
        /// Adds type constraints to the collection based on the named type.
        /// </summary>
        /// <param name="namedTypeSymbol">Named type to process.</param>
        /// <param name="typeParameterConstraintClauseSyntaxList">Collection of type parameter constraints to add to.</param>
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
                if (hasNotNullConstraint || AnyBaseHasNotNullConstraint(namedTypeSymbol, typeParameterSymbol))
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

        /// <summary>
        /// Gets the Type Arguments based on the type symbol of a base class.
        /// </summary>
        /// <param name="baseClass">Base class to check.</param>
        /// <returns>Collection of Tye Arguments.</returns>
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


                string typeName = (typeParameterSymbol.TypeKind != TypeKind.TypeParameter && typeParameterSymbol.SpecialType == SpecialType.None ? "global::" : string.Empty)
                              + typeParameterSymbol.ToDisplayString();

                yield return SyntaxFactory.ParseTypeName(typeName);
            }
        }

        /// <summary>
        /// Fluent API to get a Syntax Token List of modifiers to apply to the generated class.
        /// </summary>
        /// <param name="modifiers">List of modifiers to extend.</param>
        /// <returns>Modified syntax token list.</returns>
        protected abstract SyntaxTokenList GetClassModifiers(SyntaxTokenList modifiers);

        /// <summary>
        /// Fluent API to extend a class with generated members.
        /// </summary>
        /// <param name="members">Members collection to modify.</param>
        /// <param name="namedTypeSymbol">The named type symbol to use as a reference point for generated members.</param>
        /// <param name="desiredCommandInterface">The desired command interface to check for.</param>
        /// <param name="isDerivedType">Whether the named type symbol is derived from another type, other than base UI type for the platform.</param>
        /// <param name="controlClassFullName">Full Name of the Control Class.</param>
        /// <param name="platformName">Friendly Name for the platform.</param>
        /// <param name="makeClassesPublic">A flag indicating whether to expose the generated binding classes as public rather than internal. Set this to true if you're created a reusable library file.</param>
        /// <param name="includeObsoleteItems">Whether to include obsolete items in the generated code.</param>
        /// <returns>Modified Syntax List of Member declarations.</returns>
        protected abstract SyntaxList<MemberDeclarationSyntax> ApplyMembers(
            SyntaxList<MemberDeclarationSyntax> members,
            INamedTypeSymbol namedTypeSymbol,
            string? desiredCommandInterface,
            bool isDerivedType,
            string controlClassFullName,
            string platformName,
            bool makeClassesPublic,
            bool includeObsoleteItems,
            string? platformCommandType);

        /// <summary>
        /// Gets the class name identifier from a named type symbol.
        /// </summary>
        /// <param name="namedTypeSymbol">The named type symbol to calculate from.</param>
        /// <returns>Name of the class.</returns>
        protected abstract string GetClassNameIdentifier(INamedTypeSymbol namedTypeSymbol);

        /// <summary>
        /// Gets the constructor XML DOC summary for a constructor.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="typeParameterList"></param>
        /// <returns>XML DOC summary string.</returns>
        protected abstract string GetConstructorSummaryText(string className, TypeParameterListSyntax typeParameterList);

        /// <summary>
        /// Gets the statement syntax body for a constructor.
        /// </summary>
        /// <param name="isDerivedType">Whether the type being generated is derived.</param>
        /// <returns>Constructor body.</returns>
        protected abstract IReadOnlyCollection<StatementSyntax> GetConstructorBody(bool isDerivedType);

        /// <summary>
        /// Gets the constructor's control type name.
        /// </summary>
        /// <param name="namedTypeSymbol">Named type symbol.</param>
        /// <returns>Control Type Name.</returns>
        protected abstract string GetConstructorControlTypeName(INamedTypeSymbol namedTypeSymbol);

        /// <summary>
        /// Applies a base class declaration to a class declaration.
        /// </summary>
        /// <param name="namedTypeSymbol">The named type symbol to check for a base type.</param>
        /// <param name="baseUiElement">The core UI element for the platform.</param>
        /// <param name="controlClassFullName">Full name of the control class.</param>
        /// <param name="classDeclaration">Existing class declaration to extend.</param>
        /// <param name="platformName">Friendly Name for the UI platform.</param>
        /// <param name="rootNamespace">The root namespace to place the code in.</param>
        /// <returns>Modified Class Declaration Syntax.</returns>
        protected abstract ClassDeclarationSyntax ApplyBaseClassDeclarationSyntax(
            INamedTypeSymbol namedTypeSymbol,
            string baseUiElement,
            string controlClassFullName,
            ClassDeclarationSyntax classDeclaration,
            string platformName,
            string rootNamespace);

        /// <summary>
        /// Gets a collection of type constraint clauses.
        /// </summary>
        /// <param name="controlClassFullName">Full name of the control class.</param>
        /// <param name="namedTypeSymbol">The named type symbol to check.</param>
        /// <returns>Collection of Type Parameter constraints.</returns>
        protected abstract SyntaxList<TypeParameterConstraintClauseSyntax> GetTypeParameterConstraintClauseSyntaxes(
            string controlClassFullName,
            INamedTypeSymbol namedTypeSymbol);

        /// <summary>
        /// Gets Type Parameter Syntax.
        /// </summary>
        /// <returns>Type Parameter Syntax.</returns>
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

        private MemberDeclarationSyntax GetConstructorMethod(
            INamedTypeSymbol namedTypeSymbol,
            bool isDerivedType,
            bool makeClassesPublic,
            TypeParameterListSyntax typeParameterList)
        {
            var className = GetClassNameIdentifier(namedTypeSymbol);
            var body = GetConstructorBody(isDerivedType);

            var constructorControlTypeName = GetConstructorControlTypeName(namedTypeSymbol);

            var parameters = RoslynGenerationHelpers.GetParams(new[]
            {
                $"global::System.Linq.Expressions.Expression<global::System.Func<TView, {constructorControlTypeName}>> viewExpression",
            });

            var seperatedSyntaxList = default(SeparatedSyntaxList<ArgumentSyntax>);

            seperatedSyntaxList = seperatedSyntaxList.Add(
                SyntaxFactory.Argument(
                    SyntaxFactory.IdentifierName(SyntaxFactory.Identifier("viewExpression"))));

            var baseInitializerArgumentList = SyntaxFactory.ArgumentList(seperatedSyntaxList);

            var initializer = SyntaxFactory.ConstructorInitializer(
                SyntaxKind.BaseConstructorInitializer,
                baseInitializerArgumentList);

            var summaryText = GetConstructorSummaryText(className, typeParameterList);
            var summaryParameters = new (string paramName, string paramText)[]
            {
                ("viewExpression", "expression representing the control on the view to bind to.")
            };

            var modifiers = GetConstructorModifiers(makeClassesPublic);

            var declaration = SyntaxFactory.ConstructorDeclaration(className)
                .WithInitializer(initializer)
                .WithParameterList(parameters)
                .AddModifiers(modifiers)
                .AddBodyStatements(body.ToArray())
                .WithLeadingTrivia(XmlSyntaxFactory.GenerateSummaryComment(summaryText, summaryParameters));

            return declaration;
        }

        protected abstract SyntaxToken[] GetConstructorModifiers(bool makeClassesPublic);

        private static bool BaseClassHasNotNullConstraint(INamedTypeSymbol derivedTypeSymbol, ITypeParameterSymbol typeParameterSymbol)
        {
            var baseType = derivedTypeSymbol.BaseType;
            if (baseType == null || !baseType.IsGenericType)
                return false;

            // Find the index of the type parameter in the derived class
            int paramIndex = derivedTypeSymbol.TypeParameters.IndexOf(typeParameterSymbol);
            if (paramIndex < 0 || paramIndex >= baseType.TypeArguments.Length)
                return false;

            // Get the corresponding type parameter symbol from the base type definition
            var baseTypeDef = baseType.OriginalDefinition;
            if (paramIndex >= baseTypeDef.TypeParameters.Length)
                return false;

            var baseTypeParam = baseTypeDef.TypeParameters[paramIndex];
            return baseTypeParam.HasNotNullConstraint;
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
    }
}
