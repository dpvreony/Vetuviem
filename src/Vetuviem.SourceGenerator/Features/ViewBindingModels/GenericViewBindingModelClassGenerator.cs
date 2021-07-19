using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Vetuviem.SourceGenerator.Features.Core;

namespace Vetuviem.SourceGenerator.Features.ViewBindingModels
{
    public class GenericViewBindingModelClassGenerator : AbstractViewBindingModelClassGenerator
    {
        protected override SyntaxTokenList GetClassModifiers(SyntaxTokenList modifiers)
        {
            modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.AbstractKeyword));

            return modifiers;
        }

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
                desiredCommandInterface));

            members = members.Add(GetApplyBindingsMethod(
                namedTypeSymbol,
                isDerivedType,
                desiredCommandInterface));

            return members;
        }

        protected override string GetClassNameIdentifier(INamedTypeSymbol namedTypeSymbol)
        {
            return $"Unbound{namedTypeSymbol.Name}ViewBindingModel";
        }

        protected override string GetConstructorSummaryText(string className)
        {
            return  $"Initializes a new instance of the <see cref=\"{className}{{TView, TViewModel, TControl, TValue}}\"/> class.";
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
#pragma warning disable SA1129 // Do not use default value type constructor
                var interfaceTypesList = new SeparatedSyntaxList<BaseTypeSyntax>();
#pragma warning restore SA1129 // Do not use default value type constructor
                interfaceTypesList = interfaceTypesList.Add(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName("global::Vetuviem.Core.AbstractViewBindingModel<TView, TViewModel, TControl>")));
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
                $"global::ReactiveUI.{platformName}.ViewToViewModelBindings.{subNameSpace}.Unbound{baseClass.Name}ViewBindingModel";

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
            sep = sep.AddRange(new[] {viewForParameter, viewModelParameter, controlParameter});

            if (baseClass is {IsGenericType: true})
            {
                sep = sep.AddRange(GetTypeArgumentsFromTypeParameters(baseClass));
            }

            return sep;
        }

        private MemberDeclarationSyntax GetApplyBindingsMethod(
            INamedTypeSymbol namedTypeSymbol,
            bool isDerivedType,
            string desiredCommandInterface)
        {
            const string methodName = "ApplyBindings";
            var returnType = SyntaxFactory.ParseTypeName("void");

            var methodBody = GetApplyBindingMethodBody(
                namedTypeSymbol,
                isDerivedType,
                desiredCommandInterface);

            var parameters = RoslynGenerationHelpers.GetParams(new []
            {
                "TView view",
                "TViewModel viewModel",
                "global::System.Action<global::System.IDisposable> registerForDisposalAction",
            });

            var declaration = SyntaxFactory.MethodDeclaration(returnType, methodName)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                    SyntaxFactory.Token(SyntaxKind.OverrideKeyword))
                .WithParameterList(parameters)
                .AddBodyStatements(methodBody)
                .WithLeadingTrivia(XmlSyntaxFactory.InheritdocSyntax);
            return declaration;
        }

        private StatementSyntax[] GetApplyBindingMethodBody(
            INamedTypeSymbol namedTypeSymbol,
            bool isDerivedType,
            string desiredCommandInterface)
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
                        $"global::Vetuviem.Core.ExpressionHelpers.GetControlPropertyExpressionFromViewExpression<TView, TControl, {propType}>(VetuviemControlBindingExpression, \"{propertySymbol.Name}\")",
                    };

                var invocationStatement = RoslynGenerationHelpers.GetMethodOnPropertyOfVariableInvocationSyntax(
                    $"this",
                    propertySymbol.Name,
                    "ApplyBinding",
                    invokeArgs);

                AddInvocationStatementToRelevantCollection(
                    propertySymbol,
                    desiredCommandInterface,
                    invocationStatement,
                    commandBindingStatements,
                    oneWayBindingStatements,
                    twoWayBindingStatements);
            }

            body.AddRange(commandBindingStatements);
            body.AddRange(oneWayBindingStatements);
            body.AddRange(twoWayBindingStatements);

            return body.ToArray();
        }

        private static void AddInvocationStatementToRelevantCollection(
            IPropertySymbol prop,
            string desiredCommandInterface,
            StatementSyntax invocation,
            ICollection<StatementSyntax> commandBindingStatements,
            ICollection<StatementSyntax> oneWayBindingStatements,
            ICollection<StatementSyntax> twoWayBindingStatements)
        {
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

            if (prop.IsReadOnly)
            {
                oneWayBindingStatements.Add(invocation);
                return;
            }

            twoWayBindingStatements.Add(invocation);
        }
    }
}
