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
            Action<Diagnostic> reportDiagnosticAction,
            string desiredBaseType,
            string desiredCommandInterface)
        {
            var previouslyGeneratedClasses = new List<string>();

            foreach (var metadataReference in assembliesOfInterest)
            {
                namespaceDeclaration = CheckAssemblyForUiTypes(
                    namespaceDeclaration,
                    metadataReference,
                    compilation,
                    reportDiagnosticAction,
                    desiredBaseType,
                    previouslyGeneratedClasses,
                    desiredCommandInterface);
            }

            return namespaceDeclaration;
        }

        private NamespaceDeclarationSyntax CheckAssemblyForUiTypes(
            NamespaceDeclarationSyntax namespaceDeclaration,
            MetadataReference metadataReference,
            Compilation compilation,
            Action<Diagnostic> reportDiagnosticAction,
            string baseUiElement,
            IList<string> previouslyGeneratedClasses,
            string desiredCommandInterface)
        {
            reportDiagnosticAction(ReportDiagnostics.StartingScanOfAssembly(metadataReference));

            var assemblyOrModuleSymbol = compilation.GetAssemblyOrModuleSymbol(metadataReference);

            if (assemblyOrModuleSymbol == null)
            {
                reportDiagnosticAction(ReportDiagnostics.NoAssemblyOrModuleSybmol(metadataReference));
                return namespaceDeclaration;
            }

            var globalNamespace = GetGlobalNamespace(assemblyOrModuleSymbol);
            if (globalNamespace == null)
            {
                reportDiagnosticAction(ReportDiagnostics.NoGlobalNamespaceInAssemblyOrModule(metadataReference));
                return namespaceDeclaration;
            }

            // we skip building the global namespace as gives an empty name
            foreach (var namespaceMember in globalNamespace.GetNamespaceMembers())
            {
                var nestedDeclarationSyntax = CheckNamespaceForUiTypes(
                    namespaceMember,
                    reportDiagnosticAction,
                    baseUiElement,
                    previouslyGeneratedClasses,
                    desiredCommandInterface);

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

        private INamespaceSymbol GetGlobalNamespace(ISymbol assemblyOrModuleSymbol)
        {
            switch (assemblyOrModuleSymbol)
            {
                case IAssemblySymbol assemblySymbol:
                    return assemblySymbol.GlobalNamespace;
                case IModuleSymbol moduleSymbol:
                    return moduleSymbol.GlobalNamespace;
                default:
                    return null;
            }
        }

        private ClassDeclarationSyntax CheckTypeForUiType(
            INamedTypeSymbol namedTypeSymbol,
            Action<Diagnostic> reportDiagnosticAction,
            string baseUiElement,
            IList<string> previouslyGeneratedClasses,
            string desiredCommandInterface)
        {
            var fullName = namedTypeSymbol.GetFullName();

            try
            {
                // this is in a try catch block because roslyn sometimes throws a null ref exception.
                var accessibility = namedTypeSymbol.DeclaredAccessibility;
                if (accessibility != Accessibility.Public)
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }

            // ensure we inherit from our desired element.
            if (namedTypeSymbol.IsSealed ||
                namedTypeSymbol.IsStatic ||
                (!HasDesiredBaseType(baseUiElement, namedTypeSymbol) &&
                !fullName.Equals(baseUiElement, StringComparison.Ordinal)) ||
                previouslyGeneratedClasses.Any(pgc => pgc.Equals(fullName)))
            {
                return null;
            }

            previouslyGeneratedClasses.Add(fullName);
            reportDiagnosticAction(ReportDiagnostics.HasDesiredBaseType(baseUiElement, namedTypeSymbol));
            return ViewBindingModelClassGenerator.GenerateClass(
                namedTypeSymbol,
                baseUiElement,
                desiredCommandInterface);

        }

        private NamespaceDeclarationSyntax CheckNamespaceForUiTypes(
            INamespaceSymbol namespaceSymbol,
            Action<Diagnostic> reportDiagnosticAction,
            string baseUiElement,
            IList<string> previouslyGeneratedClasses,
            string desiredCommandInterface)
        {
            reportDiagnosticAction(ReportDiagnostics.StartingScanOfNamespace(namespaceSymbol));

            var namedTypeSymbols = namespaceSymbol.GetTypeMembers();

            var nestedMembers = new List<MemberDeclarationSyntax>(namedTypeSymbols.Length);

            foreach (var namedTypeSymbol in namedTypeSymbols)
            {
                var classDeclaration = CheckTypeForUiType(
                    namedTypeSymbol,
                    reportDiagnosticAction,
                    baseUiElement,
                    previouslyGeneratedClasses,
                    desiredCommandInterface);

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
                    baseUiElement,
                    previouslyGeneratedClasses,
                    desiredCommandInterface);

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
