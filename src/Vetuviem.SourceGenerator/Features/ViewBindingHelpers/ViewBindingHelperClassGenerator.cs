using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Vetuviem.SourceGenerator.Features.Core;

namespace Vetuviem.SourceGenerator.Features.ViewBindingHelpers
{
    public class ViewBindingHelperClassGenerator : IClassGenerator
    {
        public ClassDeclarationSyntax GenerateClass(
            INamedTypeSymbol namedTypeSymbol,
            string baseUiElement,
            string desiredCommandInterface,
            string platformName)
        {
            var controlClassFullName = namedTypeSymbol.GetFullName();

            var modifiers = SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword));


            var classDeclaration = SyntaxFactory.ClassDeclaration($"{namedTypeSymbol.Name}ViewBindingHelper");

            var applyBindingMethod = GetApplyBindingMethod(
                namedTypeSymbol,
                platformName);
            var applyBindingInternalMethod = GetApplyBindingInternalMethod(
                namedTypeSymbol,
                baseUiElement,
                platformName);

            var members = new SyntaxList<MemberDeclarationSyntax>(
                new []
                {
                    applyBindingMethod,
                    applyBindingInternalMethod
                });

            return classDeclaration
                .WithModifiers(modifiers)
                .WithMembers(members)
                .WithLeadingTrivia(XmlSyntaxFactory.GenerateSummarySeeAlsoComment(
                    "A class that contains View Bindings Helper logic for the {0}ViewBindingModel and thus the {0} control.",
                    controlClassFullName));
        }

        private MemberDeclarationSyntax GetApplyBindingMethod(INamedTypeSymbol namedTypeSymbol, string platformName)
        {
            var body = GetApplyBindingMethodBody();
            return GetApplyBindingMethodViaCommonLogic(
                namedTypeSymbol,
                platformName,
                "ApplyBinding",
                body,
                SyntaxKind.PublicKeyword);
        }

        private MemberDeclarationSyntax GetApplyBindingMethodViaCommonLogic(INamedTypeSymbol namedTypeSymbol,
            string platformName,
            string methodName,
            StatementSyntax[] methodBody,
            SyntaxKind accessKeyword)
        {
            string controlType = namedTypeSymbol.GetFullName();
            var subNameSpace =
                namedTypeSymbol.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                    .Replace("global::", string.Empty);

            var baseViewBindingModelClassName =
                $"global::ReactiveUI.{platformName}.ViewToViewModelBindings.{subNameSpace}.{namedTypeSymbol.Name}ViewBindingModel";

            // TODO: make use of the type params method, need to remove a lot of this parsing stuff
            var genericArg = namedTypeSymbol.IsGenericType ? ",T" : string.Empty;

            var parameters = RoslynGenerationHelpers.GetParams(new []
            {
                "TView view",
                "TViewModel viewModel",
                $"{baseViewBindingModelClassName}<TView, TViewModel{genericArg}> viewBindingModel",
                "global::System.Action<global::System.IDisposable> registerForDisposalAction",
                $"global::System.Linq.Expressions.Expression<global::System.Func<TView, {controlType}>> control",
            });


            var typeParameterList = GetTypeParameterListSyntax(namedTypeSymbol);
            var returnType = SyntaxFactory.ParseTypeName("void");
            var constraintClauses = GetTypeParameterConstraintClauseSyntaxes();

            var declaration = SyntaxFactory.MethodDeclaration(returnType, methodName)
                .WithTypeParameterList(typeParameterList)
                .WithConstraintClauses(constraintClauses)
                .WithParameterList(parameters)
                .AddModifiers(SyntaxFactory.Token(accessKeyword),
                    SyntaxFactory.Token(SyntaxKind.StaticKeyword))
                .AddBodyStatements(methodBody);
            return declaration;
        }

        private MemberDeclarationSyntax GetApplyBindingInternalMethod(INamedTypeSymbol namedTypeSymbol, string baseUiElement, string platformName)
        {
            var body = GetApplyBindingInternalMethodBody(namedTypeSymbol, baseUiElement, platformName);
            return GetApplyBindingMethodViaCommonLogic(
                namedTypeSymbol,
                platformName,
                "ApplyBindingInternal",
                body,
                SyntaxKind.InternalKeyword);
        }

        private StatementSyntax[] GetApplyBindingMethodBody()
        {
            var args = new[] { "view", "viewModel", "viewBindingModel", "registerForDisposalAction", "control"};

            return new StatementSyntax[]
            {
                RoslynGenerationHelpers.GetNullGuardCheckSyntax("view"),
                RoslynGenerationHelpers.GetNullGuardCheckSyntax("viewModel"),
                RoslynGenerationHelpers.GetNullGuardCheckSyntax("viewBindingModel"),
                RoslynGenerationHelpers.GetNullGuardCheckSyntax("registerForDisposalAction"),
                RoslynGenerationHelpers.GetNullGuardCheckSyntax("control"),
                SyntaxFactory.ExpressionStatement(RoslynGenerationHelpers.GetStaticMethodInvocationSyntax("ApplyBindingInternal", args, false)),
            };
        }

        private StatementSyntax[] GetApplyBindingInternalMethodBody(
            INamedTypeSymbol namedTypeSymbol,
            string baseUiElement,
            string platformName)
        {
            var body = new List<StatementSyntax>();

            var properties = namedTypeSymbol
                .GetMembers()
                .Where(x => x.Kind == SymbolKind.Property)
                .ToArray();

            // TODO: add a call to the base controls binding helper.
            // these classes are all static helpers so it's not a base.ApplyBindings()
            // it was done so not doing loads of ctor's during binding.
            AddUpstreamStaticClassInvocationIfRequired(
                body,
                namedTypeSymbol,
                baseUiElement,
                platformName);

            var controlFullName = namedTypeSymbol.GetFullName();

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

                var propType = propertySymbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                var invokeArgs = new[]
                    {
                        "registerForDisposalAction",
                        "view",
                        "viewModel",
                        $"global::Vetuviem.Core.ExpressionHelpers.GetControlPropertyExpressionFromViewExpression<TView, {controlFullName}, {propType}>(control, \"{propertySymbol.Name}\")",
                    };

                body.Add(RoslynGenerationHelpers.GetMethodOnPropertyOfVariableInvocationSyntax(
                    $"viewBindingModel",
                    propertySymbol.Name,
                    "ApplyBinding",
                    invokeArgs));
            }



            return body.ToArray();
        }

        private void AddUpstreamStaticClassInvocationIfRequired(
            List<StatementSyntax> body,
            INamedTypeSymbol namedTypeSymbol,
            string baseUiElement,
            string platformName)
        {
            var controlClassFullName = namedTypeSymbol.GetFullName();
            if (controlClassFullName.Equals(baseUiElement, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var baseClass = namedTypeSymbol.BaseType;
            if (baseClass?.BaseType == null)
            {
                // this is system.object which we don't produce a binding model for
                // this happens when digging for a ui system that uses interfaces as the base description
                // of ui components. i.e. blazor.
                return;
            }

            // we don't use the full name of the type symbol as if the class is generic you end up with the type args in it.
            var subNameSpace =
                baseClass.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                    .Replace("global::", string.Empty);

            var baseViewBindingModelClassName =
                $"global::ReactiveUI.{platformName}.ViewToViewModelBindingHelpers.{subNameSpace}.{baseClass.Name}ViewBindingHelper";

            var controlFullName = namedTypeSymbol.GetFullName();
            var baseClassFullName = baseClass.GetFullName();

            var invocationSyntax = RoslynGenerationHelpers.GetStaticMethodInvocationSyntax(
                baseViewBindingModelClassName,
                "ApplyBindingInternal",
                new[]
                {
                    "view",
                    "viewModel",
                    "viewBindingModel",
                    "registerForDisposalAction",
                    $"global::Vetuviem.Core.ExpressionHelpers.ConvertControlExpressionToBaseClassExpression<TView, {controlFullName}, {baseClassFullName}>(control)"
                },
                false);

            var invocationExpression = SyntaxFactory.ExpressionStatement(invocationSyntax);

            body.Add(invocationExpression);

        }

        private static SyntaxList<TypeParameterConstraintClauseSyntax> GetTypeParameterConstraintClauseSyntaxes()
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
            var constraintClauses =
                new SyntaxList<TypeParameterConstraintClauseSyntax>(new[] {viewConstraintClause, viewModelConstraintClause});
            return constraintClauses;
        }

        private static TypeParameterListSyntax GetTypeParameterListSyntax(INamedTypeSymbol namedTypeSymbol)
        {
            var viewForParameter = SyntaxFactory.TypeParameter("TView");
            var viewModelParameter = SyntaxFactory.TypeParameter("TViewModel");

#pragma warning disable SA1129 // Do not use default value type constructor
            var sep = new SeparatedSyntaxList<TypeParameterSyntax>();
#pragma warning restore SA1129 // Do not use default value type constructor
            sep = sep.AddRange(new[] {viewForParameter, viewModelParameter});

            if (namedTypeSymbol.IsGenericType)
            {
                sep = sep.AddRange(GetTypeParameterSeparatedSyntaxList(namedTypeSymbol));
            }

            var typeParameterList = SyntaxFactory.TypeParameterList(sep);
            return typeParameterList;
        }

        private static IEnumerable<TypeParameterSyntax> GetTypeParameterSeparatedSyntaxList(INamedTypeSymbol namedTypeSymbol)
        {
            foreach (var typeParameterSymbol in namedTypeSymbol.TypeParameters)
            {
                yield return SyntaxFactory.TypeParameter(typeParameterSymbol.Name);
            }
        }

        private static ClassDeclarationSyntax ApplyBaseClassDeclarationSyntax(
            INamedTypeSymbol namedTypeSymbol,
            string baseUiElement,
            string controlClassFullName,
            ClassDeclarationSyntax classDeclaration,
            string platformName)
        {
            if (controlClassFullName.Equals(baseUiElement, StringComparison.OrdinalIgnoreCase))
            {
                return classDeclaration;
            }

            var baseClass = namedTypeSymbol.BaseType;
            if (baseClass?.BaseType == null)
            {
                // this is system.object which we don't produce a binding model for
                // this happens when digging for a ui system that uses interfaces as the base description
                // of ui components. i.e. blazor.
                return classDeclaration;
            }

            // we dont use the full name of the type symbol as if the class is generic you end up with the type args in it.
            var subNameSpace =
                baseClass.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                    .Replace("global::", string.Empty);

            var baseViewBindingModelClassName =
                $"global::ReactiveUI.{platformName}.ViewToViewModelBindings.{subNameSpace}.{baseClass.Name}ViewBindingHelper";

            var baseTypeIdentifier = SyntaxFactory.Identifier(baseViewBindingModelClassName);

            var baseTypeName = SyntaxFactory.IdentifierName(baseTypeIdentifier);

            var baseTypeNode = SyntaxFactory.SimpleBaseType(baseTypeName);

#pragma warning disable SA1129 // Do not use default value type constructor
            var baseTypesList = new SeparatedSyntaxList<BaseTypeSyntax>();
#pragma warning restore SA1129 // Do not use default value type constructor
            baseTypesList = baseTypesList.Add(baseTypeNode);
            var baseList = SyntaxFactory.BaseList(baseTypesList);

            classDeclaration = classDeclaration.WithBaseList(baseList);

            return classDeclaration;
        }

        private static TypeArgumentListSyntax GetTypeArgumentListSyntax(INamedTypeSymbol namedTypeSymbol, INamedTypeSymbol baseClass)
        {
#pragma warning disable SA1129 // Do not use default value type constructor
            var sep = GetTypeArgumentSeparatedSyntaxList(namedTypeSymbol, baseClass);
#pragma warning restore SA1129 // Do not use default value type constructor
            var typeArgumentList = SyntaxFactory.TypeArgumentList(sep);

            return typeArgumentList;
        }

        private static SeparatedSyntaxList<TypeSyntax> GetTypeArgumentSeparatedSyntaxList(
            INamedTypeSymbol namedTypeSymbol,
            INamedTypeSymbol baseClass)
        {
            var viewForParameter = SyntaxFactory.ParseTypeName("TView");
            var viewModelParameter = SyntaxFactory.ParseTypeName("TViewModel");
#pragma warning disable SA1129 // Do not use default value type constructor
            var sep = new SeparatedSyntaxList<TypeSyntax>();
#pragma warning restore SA1129 // Do not use default value type constructor
            sep = sep.AddRange(new[] {viewForParameter, viewModelParameter});

            if (baseClass is {IsGenericType: true})
            {
                sep = sep.AddRange(GetTypeArgumentsFromTypeParameters(baseClass));
            }

            return sep;
        }

        private static IEnumerable<TypeSyntax> GetTypeArgumentsFromTypeParameters(INamedTypeSymbol baseClass)
        {
            foreach (var typeParameterSymbol in baseClass.TypeArguments)
            {
                yield return SyntaxFactory.ParseTypeName(typeParameterSymbol.ToDisplayString());
            }
        }
    }
}
