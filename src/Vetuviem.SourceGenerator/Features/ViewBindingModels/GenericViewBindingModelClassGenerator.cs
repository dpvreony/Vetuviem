using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Vetuviem.SourceGenerator.Features.Core;

namespace Vetuviem.SourceGenerator.Features.ViewBindingModels
{
    public class GenericViewBindingModelClassGenerator : AbstractViewBindingModelClassGenerator
    {
        protected override SyntaxList<MemberDeclarationSyntax> ApplyMembers(
            SyntaxList<MemberDeclarationSyntax> members,
            INamedTypeSymbol namedTypeSymbol,
            string desiredCommandInterface,
            bool isDerivedType,
            string controlClassFullName,
            string platformName)
        {
            members = members.AddRange(ViewBindingModelPropertyGenerator.GetProperties(
                namedTypeSymbol,
                desiredCommandInterface,
                isDerivedType,
                controlClassFullName));

            members = members.Add(GetApplyBindingsMethod(
                namedTypeSymbol,
                desiredCommandInterface,
                platformName));

            return members;
        }

        protected override string GetClassNameIdentifier(INamedTypeSymbol namedTypeSymbol)
        {
            return $"Unbound{namedTypeSymbol.Name}ViewBindingModel";
        }

        protected override List<StatementSyntax> GetConstructorBody(bool isDerivedType)
        {
            var body = new List<StatementSyntax>();
            return body;
        }

        protected override string GetConstructorControlTypeName(INamedTypeSymbol namedTypeSymbol)
        {
            return "TControl";
        }

        protected override SeparatedSyntaxList<TypeParameterSyntax> GetTypeParameterSyntaxes()
        {
            var viewForParameter = SyntaxFactory.TypeParameter("TView");
            var viewModelParameter = SyntaxFactory.TypeParameter("TViewModel");
            var controlParameter = SyntaxFactory.TypeParameter("TControl");

#pragma warning disable SA1129 // Do not use default value type constructor
            var sep = new SeparatedSyntaxList<TypeParameterSyntax>();
#pragma warning restore SA1129 // Do not use default value type constructor
            sep = sep.AddRange(new[] {viewForParameter, viewModelParameter, controlParameter});
            return sep;
        }

        protected override ClassDeclarationSyntax ApplyBaseClassDeclarationSyntax(
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
                interfaceTypesList = interfaceTypesList.Add(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName("global::Vetuviem.Core.AbstractViewBindingModel<TView, TViewModel, TControl>")));
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
            var controlParameter = SyntaxFactory.ParseTypeName("TControl");
#pragma warning disable SA1129 // Do not use default value type constructor
            var sep = new SeparatedSyntaxList<TypeSyntax>();
#pragma warning restore SA1129 // Do not use default value type constructor
            sep = sep.AddRange(new[] {viewForParameter, viewModelParameter, controlParameter});

            if (baseClass is {IsGenericType: true})
            {
                sep = sep.AddRange(GetTypeArgumentsFromTypeParameters(baseClass));
            }

            return sep;
        }

        private MemberDeclarationSyntax GetApplyBindingsMethod(
            INamedTypeSymbol namedTypeSymbol,
            string desiredCommandInterface,
            string platformName)
        {
            const string methodName = "ApplyBindings";
            var returnType = SyntaxFactory.ParseTypeName("void");
            var args = new[] { "view", "viewModel", "this", "registerForDisposalAction", "this.VetuviemControlBindingExpression"};
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

    }
}
