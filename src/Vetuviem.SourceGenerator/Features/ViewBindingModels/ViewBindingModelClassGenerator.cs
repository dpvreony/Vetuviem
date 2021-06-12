using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Vetuviem.SourceGenerator.Features.Core;

namespace Vetuviem.SourceGenerator.Features.ViewBindingModels
{
    public class ViewBindingModelClassGenerator : IClassGenerator
    {
        public ClassDeclarationSyntax GenerateClass(
            INamedTypeSymbol namedTypeSymbol,
            string baseUiElement,
            string desiredCommandInterface,
            string platformName)
        {
            var typeParameterList = GetTypeParameterListSyntax(namedTypeSymbol);

            var controlClassFullName = namedTypeSymbol.GetFullName();

            var modifiers = SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            var constraintClauses = GetTypeParameterConstraintClauseSyntaxes();

            var classDeclaration = SyntaxFactory.ClassDeclaration($"{namedTypeSymbol.Name}ViewBindingModel");

            classDeclaration = ApplyBaseClassDeclarationSyntax(
                namedTypeSymbol,
                baseUiElement,
                controlClassFullName,
                classDeclaration,
                platformName);

            var isDerivedType = !controlClassFullName.Equals(baseUiElement, StringComparison.OrdinalIgnoreCase);

            var members = new SyntaxList<MemberDeclarationSyntax>(GetConstructorMethod(namedTypeSymbol, controlClassFullName));

            members = members.AddRange(ViewBindingModelPropertyGenerator.GetProperties(
                namedTypeSymbol,
                desiredCommandInterface,
                isDerivedType,
                controlClassFullName));

            members = members.Add(GetApplyBindingsMethod(
                namedTypeSymbol,
                desiredCommandInterface,
                platformName));

            return classDeclaration
                .WithModifiers(modifiers)
                .WithTypeParameterList(typeParameterList)
                .WithConstraintClauses(constraintClauses)
                .WithLeadingTrivia(XmlSyntaxFactory.GenerateSummarySeeAlsoComment(
                    "A class that contains View Bindings for the {0} control.",
                    controlClassFullName))
                .WithMembers(members);
        }

        private MemberDeclarationSyntax GetConstructorMethod(
            INamedTypeSymbol namedTypeSymbol,
            string controlClassFullName)
        {
            var className = $"{namedTypeSymbol.Name}ViewBindingModel";
            var body = new List<StatementSyntax>();

            var parameters = RoslynGenerationHelpers.GetParams(new []
            {
                $"global::System.Linq.Expressions.Expression<global::System.Func<TView, {controlClassFullName}>> viewExpression",
            });

            var declaration = SyntaxFactory.ConstructorDeclaration(className)
                .WithParameterList(parameters)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddBodyStatements(body.ToArray());

            return declaration;
        }

        private MemberDeclarationSyntax GetApplyBindingsMethod(
            INamedTypeSymbol namedTypeSymbol,
            string desiredCommandInterface,
            string platformName)
        {
            const string methodName = "ApplyBindings";
            var returnType = SyntaxFactory.ParseTypeName("void");
            var args = new[] { "view", "viewModel", "this", "registerForDisposalAction"};
            var subNameSpace =
                namedTypeSymbol.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                    .Replace("global::", string.Empty);

            var baseViewBindingModelClassName =
                $"global::ReactiveUI.{platformName}.ViewToViewModelBindingHelpers.{subNameSpace}.{namedTypeSymbol.Name}ViewBindingHelper";

            var methodBody = new StatementSyntax[]
            {
                SyntaxFactory.ExpressionStatement(RoslynGenerationHelpers.GetStaticMethodInvocationSyntax(baseViewBindingModelClassName, "ApplyBinding", args, false)),
            };

            var isOverride = false;

            var parameters = RoslynGenerationHelpers.GetParams(new []
            {
                "TView view",
                "TViewModel viewModel",
                "global::System.Action<global::System.IDisposable> registerForDisposalAction",
            });

            var declaration = SyntaxFactory.MethodDeclaration(returnType, methodName)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                    SyntaxFactory.Token(isOverride ? SyntaxKind.OverrideKeyword : SyntaxKind.VirtualKeyword))
                .WithParameterList(parameters)
                .AddBodyStatements(methodBody);
            return declaration;
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
                // so we're at the core type we're generating for. so we put our interface on here.
#pragma warning disable SA1129 // Do not use default value type constructor
                var interfaceTypesList = new SeparatedSyntaxList<BaseTypeSyntax>();
#pragma warning restore SA1129 // Do not use default value type constructor
                interfaceTypesList = interfaceTypesList.Add(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName("global::Vetuviem.Core.IViewBindingModel<TView, TViewModel>")));
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
                return classDeclaration;
            }

            var typeParameters = GetTypeArgumentListSyntax(
                namedTypeSymbol,
                baseClass);

            // we dont use the full name of the type symbol as if the class is generic you end up with the type args in it.
            var subNameSpace =
                baseClass.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                    .Replace("global::", string.Empty);

            var baseViewBindingModelClassName =
                $"global::ReactiveUI.{platformName}.ViewToViewModelBindings.{subNameSpace}.{baseClass.Name}ViewBindingModel";

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
