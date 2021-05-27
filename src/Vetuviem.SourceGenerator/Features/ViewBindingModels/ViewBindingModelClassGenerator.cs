using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Vetuviem.SourceGenerator.Features.Core;

namespace Vetuviem.SourceGenerator.Features.ViewBindingModels
{
    public static class ViewBindingModelClassGenerator
    {
        public static ClassDeclarationSyntax GenerateClass(
            INamedTypeSymbol namedTypeSymbol,
            string baseUiElement,
            string desiredCommandInterface)
        {
            var typeParameterList = GetTypeParameterListSyntax();

            var controlClassFullName = namedTypeSymbol.GetFullName();

            var modifiers = SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            var constraintClauses = GetTypeParameterConstraintClauseSyntaxes();

            var classDeclaration = SyntaxFactory.ClassDeclaration($"{namedTypeSymbol.Name}ViewBindingModel");

            classDeclaration = ApplyBaseClassDeclarationSyntax(
                namedTypeSymbol,
                baseUiElement,
                controlClassFullName,
                classDeclaration);

            var properties = ViewBindingModelPropertyGenerator.GetProperties(
                namedTypeSymbol,
                desiredCommandInterface);

            return classDeclaration
                .WithModifiers(modifiers)
                .WithTypeParameterList(typeParameterList)
                .WithConstraintClauses(constraintClauses)
                .WithLeadingTrivia(XmlSyntaxFactory.GenerateSummarySeeAlsoComment(
                    "A class that contains View Bindings for the {0} control.",
                    controlClassFullName))
                .WithMembers(properties);
        }

        private static ClassDeclarationSyntax ApplyBaseClassDeclarationSyntax(INamedTypeSymbol namedTypeSymbol,
            string baseUiElement, string controlClassFullName, ClassDeclarationSyntax classDeclaration)
        {
            if (!controlClassFullName.Equals(baseUiElement, StringComparison.OrdinalIgnoreCase))
            {
                var baseClass = namedTypeSymbol.BaseType;

                var baseViewBindingModelClassName =
                    $"global::ReactiveUI.WPF.ViewToViewModelBindings.{baseClass.GetFullName().Replace("global::", string.Empty)}ViewBindingModel<TView, TViewModel>";
                var baseTypeNode =
                    SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(baseViewBindingModelClassName));
#pragma warning disable SA1129 // Do not use default value type constructor
                var baseTypesList = new SeparatedSyntaxList<BaseTypeSyntax>();
#pragma warning restore SA1129 // Do not use default value type constructor
                baseTypesList = baseTypesList.Add(baseTypeNode);
                var baseList = SyntaxFactory.BaseList(baseTypesList);

                classDeclaration = classDeclaration.WithBaseList(baseList);
            }

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

        private static TypeParameterListSyntax GetTypeParameterListSyntax()
        {
            var viewForParameter = SyntaxFactory.TypeParameter("TView");
            var viewModelParameter = SyntaxFactory.TypeParameter("TViewModel");
#pragma warning disable SA1129 // Do not use default value type constructor
            var sep = new SeparatedSyntaxList<TypeParameterSyntax>();
#pragma warning restore SA1129 // Do not use default value type constructor
            sep = sep.AddRange(new[] {viewForParameter, viewModelParameter});
            var typeParameterList = SyntaxFactory.TypeParameterList(sep);
            return typeParameterList;
        }
    }
}
