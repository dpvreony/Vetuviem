using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Vetuviem.SourceGenerator.Features.Core;

namespace Vetuviem.SourceGenerator.Features.ViewBindingModels
{
    public class ControlBoundViewBindingModelClassGenerator : AbstractViewBindingModelClassGenerator
    {
        protected override SyntaxList<MemberDeclarationSyntax> ApplyMembers(
            SyntaxList<MemberDeclarationSyntax> members,
            INamedTypeSymbol namedTypeSymbol,
            string desiredCommandInterface,
            bool isDerivedType,
            string controlClassFullName,
            string platformName)
        {
            return members;
        }

        protected override string GetClassNameIdentifier(INamedTypeSymbol namedTypeSymbol)
        {
            return $"{namedTypeSymbol.Name}ViewBindingModel";
        }

        protected override List<StatementSyntax> GetConstructorBody(bool isDerivedType)
        {
            var body = new List<StatementSyntax>();
            return body;
        }

        protected override string GetConstructorControlTypeName(INamedTypeSymbol namedTypeSymbol)
        {
            return namedTypeSymbol.GetFullName();
        }

        protected override SeparatedSyntaxList<TypeParameterSyntax> GetTypeParameterSyntaxes()
        {
            var viewForParameter = SyntaxFactory.TypeParameter("TView");
            var viewModelParameter = SyntaxFactory.TypeParameter("TViewModel");

#pragma warning disable SA1129 // Do not use default value type constructor
            var sep = new SeparatedSyntaxList<TypeParameterSyntax>();
#pragma warning restore SA1129 // Do not use default value type constructor
            sep = sep.AddRange(new[] {viewForParameter, viewModelParameter});
            return sep;
        }

        protected override ClassDeclarationSyntax ApplyBaseClassDeclarationSyntax(
            INamedTypeSymbol namedTypeSymbol,
            string baseUiElement,
            string controlClassFullName,
            ClassDeclarationSyntax classDeclaration,
            string platformName)
        {
            var typeParameters = GetTypeArgumentListSyntax(namedTypeSymbol);

            // we dont use the full name of the type symbol as if the class is generic you end up with the type args in it.
            var subNameSpace =
                namedTypeSymbol.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                    .Replace("global::", string.Empty);

            var baseViewBindingModelClassName =
                $"global::ReactiveUI.{platformName}.ViewToViewModelBindings.{subNameSpace}.Unbound{namedTypeSymbol.Name}ViewBindingModel";

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


        protected override SyntaxList<TypeParameterConstraintClauseSyntax> GetTypeParameterConstraintClauseSyntaxes(string controlClassFullName)
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

        private static TypeArgumentListSyntax GetTypeArgumentListSyntax(INamedTypeSymbol namedTypeSymbol)
        {
#pragma warning disable SA1129 // Do not use default value type constructor
            var sep = GetTypeArgumentSeparatedSyntaxList(namedTypeSymbol);
#pragma warning restore SA1129 // Do not use default value type constructor
            var typeArgumentList = SyntaxFactory.TypeArgumentList(sep);

            return typeArgumentList;
        }

        private static SeparatedSyntaxList<TypeSyntax> GetTypeArgumentSeparatedSyntaxList(
            INamedTypeSymbol namedTypeSymbol)
        {
            var viewForParameter = SyntaxFactory.ParseTypeName("TView");
            var viewModelParameter = SyntaxFactory.ParseTypeName("TViewModel");
            var controlParameter = SyntaxFactory.ParseTypeName(namedTypeSymbol.GetFullName());
#pragma warning disable SA1129 // Do not use default value type constructor
            var sep = new SeparatedSyntaxList<TypeSyntax>();
#pragma warning restore SA1129 // Do not use default value type constructor
            sep = sep.AddRange(new[] {viewForParameter, viewModelParameter, controlParameter});

            if (namedTypeSymbol is {IsGenericType: true})
            {
                sep = sep.AddRange(GetTypeArgumentsFromTypeParameters(namedTypeSymbol));
            }

            return sep;
        }
    }
}
