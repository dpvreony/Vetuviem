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
    /// <summary>
    /// Class Generator for a generic "unbound" control binding model.
    /// </summary>
    public class GenericControlBindingModelClassGenerator : AbstractControlBindingModelClassGenerator
    {
        /// <inheritdoc />
        protected override SyntaxTokenList GetClassModifiers(SyntaxTokenList modifiers)
        {
            modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.AbstractKeyword));

            return modifiers;
        }

        /// <inheritdoc />
        protected override SyntaxList<MemberDeclarationSyntax> ApplyMembers(
            SyntaxList<MemberDeclarationSyntax> members,
            INamedTypeSymbol namedTypeSymbol,
            string? desiredCommandInterface,
            bool isDerivedType,
            string controlClassFullName,
            string platformName,
            bool makeClassesPublic,
            bool includeObsoleteItems,
            string? platformCommandType)
        {
            members = members.AddRange(ControlBindingModelPropertyGenerator.GetProperties(
                namedTypeSymbol,
                desiredCommandInterface,
                makeClassesPublic,
                includeObsoleteItems,
                platformCommandType));

            members = members.Add(GetApplyBindingsWithDisposableActionMethod(
                namedTypeSymbol,
                isDerivedType,
                desiredCommandInterface,
                includeObsoleteItems,
                platformCommandType));

            members = members.Add(GetApplyBindingsWithCompositeDisposableMethod(
                namedTypeSymbol,
                isDerivedType,
                desiredCommandInterface,
                includeObsoleteItems,
                platformCommandType));

            return members;
        }

        /// <inheritdoc />
        protected override string GetClassNameIdentifier(INamedTypeSymbol namedTypeSymbol)
        {
            if (namedTypeSymbol == null)
            {
                throw new ArgumentNullException(nameof(namedTypeSymbol));
            }

            return $"AbstractUnbound{namedTypeSymbol.Name}ControlBindingModel";
        }

        /// <inheritdoc />
        protected override string GetConstructorSummaryText(string className, TypeParameterListSyntax typeParameterList)
        {
            var typeParams = string.Join(", ", typeParameterList.Parameters.Select(s => s.Identifier.ValueText));
            return $"Initializes a new instance of the <see cref=\"{className}{{{typeParams}}}\"/> class.";
        }

        /// <inheritdoc />
        protected override IReadOnlyCollection<StatementSyntax> GetConstructorBody(bool isDerivedType)
        {
            var body = new List<StatementSyntax>();
            return body;
        }

        /// <inheritdoc />
        protected override string GetConstructorControlTypeName(INamedTypeSymbol namedTypeSymbol)
        {
            return "TControl";
        }

        /// <inheritdoc />
        protected override SeparatedSyntaxList<TypeParameterSyntax> GetTypeParameterSyntaxes()
        {
            var viewForParameter = SyntaxFactory.TypeParameter("TView");
            var viewModelParameter = SyntaxFactory.TypeParameter("TViewModel");
            var controlParameter = SyntaxFactory.TypeParameter("TControl");

#pragma warning disable SA1129 // Do not use default value type constructor
            var sep = new SeparatedSyntaxList<TypeParameterSyntax>();
#pragma warning restore SA1129 // Do not use default value type constructor
            sep = sep.AddRange(new[] { viewForParameter, viewModelParameter, controlParameter });
            return sep;
        }

        /// <inheritdoc />
        protected override SyntaxToken[] GetConstructorModifiers(bool _) =>
            [SyntaxFactory.Token(SyntaxKind.ProtectedKeyword)];

        /// <inheritdoc />
        protected override ClassDeclarationSyntax ApplyBaseClassDeclarationSyntax(
            INamedTypeSymbol namedTypeSymbol,
            string baseUiElement,
            string controlClassFullName,
            ClassDeclarationSyntax classDeclaration,
            string platformName,
            string rootNamespace)
        {
            if (namedTypeSymbol == null)
            {
                throw new ArgumentNullException(nameof(namedTypeSymbol));
            }

            if (classDeclaration == null)
            {
                throw new ArgumentNullException(nameof(classDeclaration));
            }

            if (string.IsNullOrWhiteSpace(controlClassFullName))
            {
                throw new ArgumentNullException(nameof(controlClassFullName));
            }

            if (controlClassFullName.Equals(baseUiElement, StringComparison.OrdinalIgnoreCase))
            {
                // so we're at the core type we're generating for. so we put our interface on here.
#pragma warning disable SA1129 // Do not use default value type constructor
                var interfaceTypesList = new SeparatedSyntaxList<BaseTypeSyntax>();
#pragma warning restore SA1129 // Do not use default value type constructor
                interfaceTypesList = interfaceTypesList.Add(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName("global::Vetuviem.Core.AbstractControlBindingModel<TView, TViewModel, TControl>")));
                var interfaceList = SyntaxFactory.BaseList(interfaceTypesList);
                classDeclaration = classDeclaration.WithBaseList(interfaceList);

                return classDeclaration;
            }

            var baseClass = namedTypeSymbol.BaseType;
            if (baseClass?.BaseType == null)
            {
                // this is system.object which we don't produce a binding model for
                // this happens when digging for a ui system that uses interfaces as the base description
                // of ui components. i.e. blazor.
#pragma warning disable SA1129 // Do not use default value type constructor
                var interfaceTypesList = new SeparatedSyntaxList<BaseTypeSyntax>();
#pragma warning restore SA1129 // Do not use default value type constructor
                interfaceTypesList = interfaceTypesList.Add(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName("global::Vetuviem.Core.AbstractControlBindingModel<TView, TViewModel, TControl>")));
                var interfaceList = SyntaxFactory.BaseList(interfaceTypesList);
                classDeclaration = classDeclaration.WithBaseList(interfaceList);

                return classDeclaration;
            }

            var typeParameters = GetTypeArgumentListSyntax(baseClass);

            // we dont use the full name of the type symbol as if the class is generic you end up with the type args in it.
            var subNameSpace =
                baseClass.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                    .Replace("global::", string.Empty);

            var baseViewBindingModelClassName =
                $"global::{rootNamespace}.{subNameSpace}.AbstractUnbound{baseClass.Name}ControlBindingModel";

            var baseTypeIdentifier = SyntaxFactory.Identifier(baseViewBindingModelClassName);

            var baseTypeName = SyntaxFactory.GenericName(
                baseTypeIdentifier,
                typeParameters);

            var baseTypeNode = SyntaxFactory.SimpleBaseType(baseTypeName);

#pragma warning disable SA1129 // Do not use default value type constructor
            var baseTypesList = new SeparatedSyntaxList<BaseTypeSyntax>();
#pragma warning restore SA1129 // Do not use default value type constructor
            baseTypesList = baseTypesList.Add(baseTypeNode);
            var baseList = SyntaxFactory.BaseList(baseTypesList);

            classDeclaration = classDeclaration.WithBaseList(baseList);

            return classDeclaration;
        }

        /// <inheritdoc />
        protected override SyntaxList<TypeParameterConstraintClauseSyntax> GetTypeParameterConstraintClauseSyntaxes(
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

            var baseControlConstraint =
                SyntaxFactory.TypeConstraint(SyntaxFactory.ParseTypeName(controlClassFullName));
#pragma warning disable SA1129 // Do not use default value type constructor
            var controlConstraints = new SeparatedSyntaxList<TypeParameterConstraintSyntax>();
#pragma warning restore SA1129 // Do not use default value type constructor
            controlConstraints =
                controlConstraints
                    .Add(baseControlConstraint);

            var controlConstraintCluase = SyntaxFactory.TypeParameterConstraintClause(
                SyntaxFactory.IdentifierName("TControl"),
                controlConstraints);

            var typeParameterConstraintClauseSyntaxList =
                new List<TypeParameterConstraintClauseSyntax>
                {
                    viewConstraintClause, viewModelConstraintClause, controlConstraintCluase
                };

            ApplyTypeConstraintsFromNamedTypedSymbol(
                namedTypeSymbol,
                typeParameterConstraintClauseSyntaxList);

            var constraintClauses =
                new SyntaxList<TypeParameterConstraintClauseSyntax>(typeParameterConstraintClauseSyntaxList);
            return constraintClauses;
        }

        private static TypeArgumentListSyntax GetTypeArgumentListSyntax(INamedTypeSymbol baseClass)
        {
#pragma warning disable SA1129 // Do not use default value type constructor
            var sep = GetTypeArgumentSeparatedSyntaxList(baseClass);
#pragma warning restore SA1129 // Do not use default value type constructor
            var typeArgumentList = SyntaxFactory.TypeArgumentList(sep);

            return typeArgumentList;
        }

        private static SeparatedSyntaxList<TypeSyntax> GetTypeArgumentSeparatedSyntaxList(INamedTypeSymbol baseClass)
        {
            var viewForParameter = SyntaxFactory.ParseTypeName("TView");
            var viewModelParameter = SyntaxFactory.ParseTypeName("TViewModel");
            var controlParameter = SyntaxFactory.ParseTypeName("TControl");
#pragma warning disable SA1129 // Do not use default value type constructor
            var sep = new SeparatedSyntaxList<TypeSyntax>();
#pragma warning restore SA1129 // Do not use default value type constructor
            sep = sep.AddRange(new[] { viewForParameter, viewModelParameter, controlParameter });

            if (baseClass is { IsGenericType: true })
            {
                sep = sep.AddRange(GetTypeArgumentsFromTypeParameters(baseClass));
            }

            return sep;
        }

        private static MemberDeclarationSyntax GetApplyBindingsWithDisposableActionMethod(
            INamedTypeSymbol namedTypeSymbol,
            bool isDerivedType,
            string? desiredCommandInterface,
            bool includeObsoleteItems,
            string? platformCommandType)
        {
            const string methodName = "ApplyBindings";
            var returnType = SyntaxFactory.ParseTypeName("void");

            var methodBody = GetApplyBindingMethodBody(
                namedTypeSymbol,
                isDerivedType,
                desiredCommandInterface,
                includeObsoleteItems,
                platformCommandType);

            var parameters = RoslynGenerationHelpers.GetParams(new[]
            {
                "TView view",
                "TViewModel viewModel",
                "global::System.Action<global::System.IDisposable> registerForDisposalAction",
            });

            // TODO: allow overrding public \ internal
            var declaration = SyntaxFactory.MethodDeclaration(returnType, methodName)
                .AddModifiers(
                    SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                    SyntaxFactory.Token(SyntaxKind.OverrideKeyword))
                .WithParameterList(parameters)
                .AddBodyStatements(methodBody)
                .WithLeadingTrivia(XmlSyntaxFactory.InheritdocSyntax);
            return declaration;
        }

        private static MemberDeclarationSyntax GetApplyBindingsWithCompositeDisposableMethod(
            INamedTypeSymbol namedTypeSymbol,
            bool isDerivedType,
            string? desiredCommandInterface,
            bool includeObsoleteItems,
            string? platformCommandType)
        {
            const string methodName = "ApplyBindings";
            var returnType = SyntaxFactory.ParseTypeName("void");

            var methodBody = GetApplyBindingCompositeDisposableMethodBody(
                namedTypeSymbol,
                isDerivedType,
                desiredCommandInterface,
                includeObsoleteItems,
                platformCommandType);

            var parameters = RoslynGenerationHelpers.GetParams(new[]
            {
                "TView view",
                "TViewModel viewModel",
                "global::System.Reactive.Disposables.CompositeDisposable compositeDisposable",
            });

            // TODO: allow overrding public \ internal
            var declaration = SyntaxFactory.MethodDeclaration(returnType, methodName)
                .AddModifiers(
                    SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                    SyntaxFactory.Token(SyntaxKind.OverrideKeyword))
                .WithParameterList(parameters)
                .AddBodyStatements(methodBody)
                .WithLeadingTrivia(XmlSyntaxFactory.InheritdocSyntax);
            return declaration;
        }

        private static StatementSyntax[] GetApplyBindingMethodBody(INamedTypeSymbol namedTypeSymbol,
            bool isDerivedType,
            string? desiredCommandInterface,
            bool includeObsoleteItems,
            string? platformCommandType)
        {
            var body = new List<StatementSyntax>();

            var properties = namedTypeSymbol
                .GetMembers()
                .Where(x => x.Kind == SymbolKind.Property)
                .ToArray();

            if (isDerivedType)
            {
                var baseInvokeArgs = new[]
                {
                    "view",
                    "viewModel",
                    "registerForDisposalAction",
                };
                body.Add(SyntaxFactory.ExpressionStatement(RoslynGenerationHelpers.GetMethodOnVariableInvocationExpression("base", "ApplyBindings", baseInvokeArgs, false)));
            }

            var controlFullName = namedTypeSymbol.GetFullName();

            var commandBindingStatements = new List<StatementSyntax>(properties.Length);
            var oneWayBindingStatements = new List<StatementSyntax>(properties.Length);
            var twoWayBindingStatements = new List<StatementSyntax>(properties.Length);

            if (!string.IsNullOrWhiteSpace(desiredCommandInterface)
                && !string.IsNullOrWhiteSpace(platformCommandType)
                && namedTypeSymbol.Interfaces.Any(interfaceName => interfaceName.GetFullName().Equals(desiredCommandInterface, StringComparison.Ordinal)))
            {
                var invokeArgs = new[]
                {
                    "registerForDisposalAction",
                    "view",
                    "viewModel",
                    "VetuviemControlBindingExpression",
                };

                var invocationStatement = RoslynGenerationHelpers.GetMethodOnPropertyOfVariableInvocationSyntax(
                    $"this",
                    "BindCommand",
                    "ApplyBinding",
                    invokeArgs);

                commandBindingStatements.Add(invocationStatement);
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

                var propType = propertySymbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                var invokeArgs = new[]
                    {
                        "registerForDisposalAction",
                        "view",
                        "viewModel",
                        $"global::Vetuviem.Core.ExpressionHelpers.GetControlPropertyExpressionFromViewExpression<TView, TControl, {propType}>(VetuviemControlBindingExpression, \"{propertySymbol.Name}\")",
                    };

                var invocationStatement = RoslynGenerationHelpers.GetMethodOnPropertyOfVariableInvocationSyntax(
                    $"this",
                    $"@{propertySymbol.Name}",
                    "ApplyBinding",
                    invokeArgs);

                AddInvocationStatementToRelevantCollection(
                    propertySymbol,
                    invocationStatement,
                    oneWayBindingStatements,
                    twoWayBindingStatements);
            }

            body.AddRange(commandBindingStatements);
            body.AddRange(oneWayBindingStatements);
            body.AddRange(twoWayBindingStatements);

            return body.ToArray();
        }

        private static StatementSyntax[] GetApplyBindingCompositeDisposableMethodBody(INamedTypeSymbol namedTypeSymbol,
            bool isDerivedType,
            string? desiredCommandInterface,
            bool includeObsoleteItems,
            string? platformCommandType)
        {
            var body = new List<StatementSyntax>();

            var properties = namedTypeSymbol
                .GetMembers()
                .Where(x => x.Kind == SymbolKind.Property)
                .ToArray();

            if (isDerivedType)
            {
                var baseInvokeArgs = new[]
                {
                    "view",
                    "viewModel",
                    "compositeDisposable",
                };
                body.Add(SyntaxFactory.ExpressionStatement(RoslynGenerationHelpers.GetMethodOnVariableInvocationExpression("base", "ApplyBindings", baseInvokeArgs, false)));
            }

            var commandBindingStatements = new List<StatementSyntax>(properties.Length);
            var oneWayBindingStatements = new List<StatementSyntax>(properties.Length);
            var twoWayBindingStatements = new List<StatementSyntax>(properties.Length);

            if (!string.IsNullOrWhiteSpace(desiredCommandInterface)
                && !string.IsNullOrWhiteSpace(platformCommandType)
                && namedTypeSymbol.Interfaces.Any(interfaceName => interfaceName.GetFullName().Equals(desiredCommandInterface, StringComparison.Ordinal)))
            {
                var invokeArgs = new[]
                {
                    "compositeDisposable",
                    "view",
                    "viewModel",
                    "VetuviemControlBindingExpression",
                };

                var invocationStatement = RoslynGenerationHelpers.GetMethodOnPropertyOfVariableInvocationSyntax(
                    $"this",
                    "BindCommand",
                    "ApplyBinding",
                    invokeArgs);

                commandBindingStatements.Add(invocationStatement);
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

                var invocationStatement = GetInvocationStatement(propertySymbol);

                AddInvocationStatementToRelevantCollection(
                    propertySymbol,
                    invocationStatement,
                    oneWayBindingStatements,
                    twoWayBindingStatements);
            }

            body.AddRange(commandBindingStatements);
            body.AddRange(oneWayBindingStatements);
            body.AddRange(twoWayBindingStatements);

            return body.ToArray();
        }

        private static StatementSyntax GetInvocationStatement(IPropertySymbol propertySymbol)
        {
            var propType = propertySymbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            var invokeArgs = new[]
            {
                "compositeDisposable",
                "view",
                "viewModel",
                $"global::Vetuviem.Core.ExpressionHelpers.GetControlPropertyExpressionFromViewExpression<TView, TControl, {propType}>(VetuviemControlBindingExpression, \"{propertySymbol.Name}\")",
            };

            var invocationStatement = RoslynGenerationHelpers.GetMethodOnPropertyOfVariableInvocationSyntax(
                $"this",
                $"@{propertySymbol.Name}",
                "ApplyBinding",
                invokeArgs);
            return invocationStatement;
        }

        private static void AddInvocationStatementToRelevantCollection(
            IPropertySymbol prop,
            StatementSyntax invocation,
            ICollection<StatementSyntax> oneWayBindingStatements,
            ICollection<StatementSyntax> twoWayBindingStatements)
        {
            /*
            if (!string.IsNullOrWhiteSpace(desiredCommandInterface))
            {
                var propType = prop.Type;
                var isCommand = propType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Equals(desiredCommandInterface, StringComparison.Ordinal)
                                || propType.AllInterfaces.Any(interfaceName => interfaceName.GetFullName().Equals(desiredCommandInterface, StringComparison.Ordinal));
                if (isCommand)
                {
                    commandBindingStatements.Add(invocation);
                    return;
                }
            }
            */

            if (prop.IsReadOnly)
            {
                oneWayBindingStatements.Add(invocation);
                return;
            }

            twoWayBindingStatements.Add(invocation);
        }
    }
}
