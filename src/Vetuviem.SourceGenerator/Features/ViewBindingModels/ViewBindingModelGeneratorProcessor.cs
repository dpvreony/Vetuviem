using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Vetuviem.SourceGenerator.Features.Core;
using Vetuviem.SourceGenerator.Features.ViewBindingModels;

namespace Vetuviem.SourceGenerator.GeneratorProcessors
{
    public sealed class ViewBindingModelGeneratorProcessor : AbstractGeneratorProcessor
    {
        public override NamespaceDeclarationSyntax GenerateObjects(
            NamespaceDeclarationSyntax namespaceDeclaration,
            MetadataReference[] assembliesOfInterest,
            Compilation compilation,
            Action<Diagnostic> reportDiagnosticAction)
        {
            var desiredBaseType = "global::System.Windows.UIElement";

            foreach (var metadataReference in assembliesOfInterest)
            {
                namespaceDeclaration = CheckAssemblyForUiTypes(
                    namespaceDeclaration,
                    metadataReference,
                    compilation,
                    reportDiagnosticAction,
                    desiredBaseType);
            }

            return namespaceDeclaration;
        }

        private NamespaceDeclarationSyntax CheckAssemblyForUiTypes(
            NamespaceDeclarationSyntax namespaceDeclaration,
            MetadataReference metadataReference,
            Compilation compilation,
            Action<Diagnostic> reportDiagnosticAction,
            string baseUiElement)
        {
            reportDiagnosticAction(ReportDiagnostics.StartingScanOfAssembly(metadataReference));

            var assemblySymbol = compilation.GetAssemblyOrModuleSymbol(metadataReference) as IAssemblySymbol;

            if (assemblySymbol == null)
            {
                reportDiagnosticAction(ReportDiagnostics.MetadataReferenceNotAssemblySymbol(metadataReference));
                return namespaceDeclaration;
            }

            var globalNamespace = assemblySymbol.GlobalNamespace;

            // we skip building the global namespace as gives an empty name
            foreach (var namespaceMember in globalNamespace.GetNamespaceMembers())
            {
                var nestedDeclarationSyntax = CheckNamespaceForUiTypes(
                    namespaceMember,
                    reportDiagnosticAction,
                    baseUiElement);

                if (nestedDeclarationSyntax != null)
                {
                    namespaceDeclaration = namespaceDeclaration
                        .AddMembers(nestedDeclarationSyntax);
                }
            }


            /*
            var typesInAssembly = assemblySymbol.get;
            foreach (string currentType in typesInAssembly)
            {
                CheckTypeForUiType(
                    assemblySymbol,
                    currentType,
                    reportDiagnosticAction);
            }
            */

            return namespaceDeclaration;
        }

        private ClassDeclarationSyntax CheckTypeForUiType(
            INamedTypeSymbol namedTypeSymbol,
            Action<Diagnostic> reportDiagnosticAction,
            string baseUiElement)
        {
            var fullName = namedTypeSymbol.GetFullName();

            // check if we inherit from our desired element.
            if (HasDesiredBaseType(baseUiElement, namedTypeSymbol))
            {
                reportDiagnosticAction(ReportDiagnostics.HasDesiredBaseType(baseUiElement, namedTypeSymbol));
                return ViewBindingModelClassGenerator.GenerateClass(namedTypeSymbol, baseUiElement);
            }

            if (fullName.Equals(baseUiElement, StringComparison.Ordinal))
            {
                reportDiagnosticAction(ReportDiagnostics.MatchedBaseUiElement(baseUiElement));
                return ViewBindingModelClassGenerator.GenerateClass(namedTypeSymbol, baseUiElement);
            }

            return null;
        }

        private NamespaceDeclarationSyntax CheckNamespaceForUiTypes(
            INamespaceSymbol namespaceSymbol,
            Action<Diagnostic> reportDiagnosticAction,
            string baseUiElement)
        {
            reportDiagnosticAction(ReportDiagnostics.StartingScanOfNamespace(namespaceSymbol));

            var namedTypeSymbols = namespaceSymbol.GetTypeMembers();

            var nestedMembers = new List<MemberDeclarationSyntax>(namedTypeSymbols.Length);

            foreach (var namedTypeSymbol in namedTypeSymbols)
            {
                var classDeclaration = CheckTypeForUiType(
                    namedTypeSymbol,
                    reportDiagnosticAction,
                    baseUiElement);

                if (classDeclaration != null)
                {
                    nestedMembers.Add(classDeclaration);
                }
            }

            var nestedSymbols = namespaceSymbol.GetNamespaceMembers();

            foreach (var nestedNamespaceSymbol in nestedSymbols)
            {
                var nestedNamespace = CheckNamespaceForUiTypes(
                    nestedNamespaceSymbol,
                    reportDiagnosticAction,
                    baseUiElement);

                if (nestedNamespace != null)
                {
                    nestedMembers.Add(nestedNamespace);
                }
            }

            if (nestedMembers.Count > 0)
            {
                var identifier = SyntaxFactory.IdentifierName(namespaceSymbol.Name);

                var membersToAdd = new SyntaxList<MemberDeclarationSyntax>(nestedMembers);

                return SyntaxFactory.NamespaceDeclaration(identifier)
                    .WithMembers(membersToAdd);
            }

            return null;
        }

        private bool HasDesiredBaseType(string desiredBaseType, INamedTypeSymbol namedTypeSymbol)
        {
            var baseType = namedTypeSymbol.BaseType;

            while (baseType != null)
            {
                var baseTypeFullName = baseType.GetFullName();
                if (baseTypeFullName.Equals(desiredBaseType, StringComparison.Ordinal))
                {
                    return true;
                }

                if (baseTypeFullName.Equals("global::System.Object", StringComparison.Ordinal))
                {
                    return false;
                }

                baseType = baseType.BaseType;
            }

            return false;
        }
    }
}
