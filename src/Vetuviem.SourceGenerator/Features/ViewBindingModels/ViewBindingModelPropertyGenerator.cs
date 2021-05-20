using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.Decompiler.TypeSystem;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pharmacist.Core.BindingModels;
using Pharmacist.Core.Generation;
using Vetuviem.SourceGenerator.Features.Core;

namespace Vetuviem.SourceGenerator.Features.ViewBindingModels
{
    /// <summary>
    /// Generates a ViewBindingModel for the discovered types.
    /// </summary>
    public sealed class ViewBindingModelPropertyGenerator : IPropertyGenerator
    {
        /// <inheritdoc/>
        public IEnumerable<NamespaceDeclarationSyntax> Generate(IEnumerable<(ITypeDefinition typeDefinition, ITypeDefinition? baseDefinition, IEnumerable<IProperty> properties)> values)
        {
            foreach (var groupedDeclarations in values.GroupBy(x => x.typeDefinition.Namespace).OrderBy(x => x.Key))
            {
                var namespaceName = $"ReactiveUI.ViewBindingModels.{groupedDeclarations.Key}";
                var members = new List<ClassDeclarationSyntax>();

                var orderedTypeDeclarations = groupedDeclarations.OrderBy(x => x.typeDefinition.Name).ToList();

                foreach (var orderedTypeDeclaration in orderedTypeDeclarations)
                {
                    members.Add(GenerateClass("System.Windows.UIElement", orderedTypeDeclaration));
                }

                if (members.Count > 0)
                {
                    yield return SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(namespaceName))
                        .WithMembers(SyntaxFactory.List<MemberDeclarationSyntax>(members));
                }
            }
        }

        private static ClassDeclarationSyntax GenerateClass(
            string baseUiElement,
            (ITypeDefinition typeDefinition, ITypeDefinition? baseDefinition, IEnumerable<IProperty> properties) orderedTypeDeclaration)
        {
            var viewForParameter = SyntaxFactory.TypeParameter("TView");
            var viewModelParameter = SyntaxFactory.TypeParameter("TViewModel");
#pragma warning disable SA1129 // Do not use default value type constructor
            var sep = new SeparatedSyntaxList<TypeParameterSyntax>();
#pragma warning restore SA1129 // Do not use default value type constructor
            sep = sep.AddRange(new[] { viewForParameter, viewModelParameter });
            var typeParameterList = SyntaxFactory.TypeParameterList(sep);

            var controlClassFullName = orderedTypeDeclaration.typeDefinition.FullName;

            var modifiers = SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

#pragma warning disable SA1129 // Do not use default value type constructor
            var viewConstraints = new SeparatedSyntaxList<TypeParameterConstraintSyntax>();
#pragma warning restore SA1129 // Do not use default value type constructor
            var viewForConstraint = SyntaxFactory.TypeConstraint(SyntaxFactory.ParseTypeName("ReactiveUI.IViewFor<TViewModel>"));

            viewConstraints = viewConstraints
                .Add(SyntaxFactory.ClassOrStructConstraint(SyntaxKind.ClassConstraint))
                .Add(viewForConstraint);
            var viewConstraintClause = SyntaxFactory.TypeParameterConstraintClause(
                SyntaxFactory.IdentifierName("TView"),
                viewConstraints);

            var reactiveObjectInterfaceConstraint = SyntaxFactory.TypeConstraint(SyntaxFactory.ParseTypeName("ReactiveUI.IReactiveObject"));
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
            var constraintClauses = new SyntaxList<TypeParameterConstraintClauseSyntax>(new[] { viewConstraintClause, viewModelConstraintClause });

            var td = orderedTypeDeclaration.typeDefinition;

            var classDeclaration = SyntaxFactory.ClassDeclaration($"{td.Name}ViewBindingModel");

            if (!td.FullName.Equals(baseUiElement, StringComparison.OrdinalIgnoreCase))
            {
                var baseClass = td.DirectBaseTypes.FirstOrDefault(x => x.Kind == ICSharpCode.Decompiler.TypeSystem.TypeKind.Class);

                var baseViewBindingModelClassName = $"global::ReactiveUI.ViewBindingModels.{baseClass.FullName}ViewBindingModel<TView, TViewModel>";
                var baseTypeNode =
                    SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(baseViewBindingModelClassName));
#pragma warning disable SA1129 // Do not use default value type constructor
                var baseTypesList = new SeparatedSyntaxList<BaseTypeSyntax>();
#pragma warning restore SA1129 // Do not use default value type constructor
                baseTypesList = baseTypesList.Add(baseTypeNode);
                var baseList = SyntaxFactory.BaseList(baseTypesList);

                classDeclaration = classDeclaration.WithBaseList(baseList);
            }

            return classDeclaration
                .WithModifiers(modifiers)
                .WithTypeParameterList(typeParameterList)
                .WithConstraintClauses(constraintClauses)
                .WithLeadingTrivia(XmlSyntaxFactory.GenerateSummarySeeAlsoComment(
                    "A class that contains View Bindings for the {0} control.",
                    $"global::{controlClassFullName}"))
                .WithMembers(GetProperties(
                    controlClassFullName,
                    orderedTypeDeclaration.properties.ToArray()));
        }

        private static SyntaxList<MemberDeclarationSyntax> GetProperties(
            string controlClassFullName,
            IProperty[] properties)
        {
            var nodes = new List<MemberDeclarationSyntax>(properties.Length);

            foreach (var prop in properties)
            {
                var accessorList = new[]
                {
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                };

                var summary = XmlSyntaxFactory.GenerateSummarySeeAlsoComment(
                    "Gets or sets the binding logic for {0}",
                    $"global::{controlClassFullName}.{prop.Name}");

                var propSyntax = GetPropertyDeclaration(prop, accessorList, summary);

                nodes.Add(propSyntax);
            }

            return new SyntaxList<MemberDeclarationSyntax>(nodes);
        }

        private static PropertyDeclarationSyntax GetPropertyDeclaration(
            IProperty prop,
            AccessorDeclarationSyntax[] accessorList,
            IEnumerable<SyntaxTrivia> summary)
        {
            var bindingType = prop.CanSet ? "OneOrTwo" : "One";

            var returnType = prop.ReturnType.GenerateFullGenericName();

            var type = SyntaxFactory.ParseTypeName($"ReactiveUI.Core.ViewBindingModels.I{bindingType}WayBind<TViewModel, {returnType}>");

            var result = SyntaxFactory.PropertyDeclaration(type, prop.Name)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            result = result
                .WithAccessorList(
                    SyntaxFactory.AccessorList(SyntaxFactory.List(accessorList)))
                .WithLeadingTrivia(summary);

            return result;
        }
    }
}
