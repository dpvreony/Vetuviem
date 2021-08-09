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


                var constraintTypes = typeParameterSymbol.ConstraintTypes;
                foreach (var constraintType in constraintTypes)
                {
                    typeParameterConstraintSyntaxList.Add(SyntaxFactory.TypeConstraint(
                                SyntaxFactory.ParseTypeName(constraintType.ToDisplayString(
                                    SymbolDisplayFormat.FullyQualifiedFormat))));
                }

#if TODO
                var constraintNullableAnnotations = typeParameterSymbol.ConstraintNullableAnnotations;
                var hasConstructorConstraint = typeParameterSymbol.HasConstructorConstraint;
                var hasNotNullConstraint = typeParameterSymbol.HasNotNullConstraint;
                var hasUnmanagedTypeConstraint = typeParameterSymbol.HasUnmanagedTypeConstraint;
                var hasValueTypeConstraint = typeParameterSymbol.HasValueTypeConstraint;

                var referenceTypeConstraintNullableAnnotation = typeParameterSymbol.ReferenceTypeConstraintNullableAnnotation;
                if (referenceTypeConstraintNullableAnnotation == NullableAnnotation.Annotated)
                {
                    newTypeParameterContraint =
                        newTypeParameterContraint
                            .Add(SyntaxFactory.ClassOrStructConstraint(SyntaxKind.ClassConstraint));
                }
#endif
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
