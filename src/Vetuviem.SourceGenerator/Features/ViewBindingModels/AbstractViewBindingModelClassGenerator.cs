using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Vetuviem.SourceGenerator.Features.Core;

namespace Vetuviem.SourceGenerator.Features.ViewBindingModels
{
    public abstract class AbstractViewBindingModelClassGenerator : IClassGenerator
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
            modifiers = GetClassModifiers(modifiers);

            var constraintClauses = GetTypeParameterConstraintClauseSyntaxes(controlClassFullName);

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
                    "A class that contains View Bindings for the {0} control.",
                    controlClassFullName))
                .WithMembers(members);
        }

        protected abstract SyntaxTokenList GetClassModifiers(SyntaxTokenList modifiers);

        protected abstract SyntaxList<MemberDeclarationSyntax> ApplyMembers(SyntaxList<MemberDeclarationSyntax> members, INamedTypeSymbol namedTypeSymbol, string desiredCommandInterface, bool isDerivedType, string controlClassFullName, string platformName);

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


            var declaration = SyntaxFactory.ConstructorDeclaration(className)
                .WithInitializer(initializer)
                .WithParameterList(parameters)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddBodyStatements(body.ToArray());

            return declaration;
        }

        protected abstract List<StatementSyntax> GetConstructorBody(bool isDerivedType);

        protected abstract string GetConstructorControlTypeName(INamedTypeSymbol namedTypeSymbol);

        protected abstract ClassDeclarationSyntax ApplyBaseClassDeclarationSyntax(
            INamedTypeSymbol namedTypeSymbol,
            string baseUiElement,
            string controlClassFullName,
            ClassDeclarationSyntax classDeclaration,
            string platformName);

        protected abstract SyntaxList<TypeParameterConstraintClauseSyntax> GetTypeParameterConstraintClauseSyntaxes(
            string controlClassFullName);

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
                yield return SyntaxFactory.TypeParameter(typeParameterSymbol.Name);
            }
        }

        protected static IEnumerable<TypeSyntax> GetTypeArgumentsFromTypeParameters(INamedTypeSymbol baseClass)
        {
            foreach (var typeParameterSymbol in baseClass.TypeArguments)
            {
                yield return SyntaxFactory.ParseTypeName(typeParameterSymbol.ToDisplayString());
            }
        }
    }
}
