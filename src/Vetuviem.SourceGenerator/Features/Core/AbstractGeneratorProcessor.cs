using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Vetuviem.SourceGenerator.Features.Core;

namespace Vetuviem.SourceGenerator.GeneratorProcessors
{
    public abstract class AbstractGeneratorProcessor
    {
        public NamespaceDeclarationSyntax GenerateObjects(
            NamespaceDeclarationSyntax namespaceDeclaration,
            MetadataReference[] assembliesOfInterest,
            Compilation compilation,
            Action<Diagnostic> reportDiagnosticAction,
            string desiredBaseType,
            bool desiredBaseTypeIsInterface,
            string desiredCommandInterface,
            string platformName)
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
                    desiredBaseTypeIsInterface,
                    previouslyGeneratedClasses,
                    desiredCommandInterface,
                    platformName);
            }

            return namespaceDeclaration;
        }

        protected abstract Func<IClassGenerator>[] GetClassGenerators();

        private NamespaceDeclarationSyntax CheckAssemblyForUiTypes(
            NamespaceDeclarationSyntax namespaceDeclaration,
            MetadataReference metadataReference,
            Compilation compilation,
            Action<Diagnostic> reportDiagnosticAction,
            string baseUiElement,
            bool desiredBaseTypeIsInterface,
            IList<string> previouslyGeneratedClasses,
            string desiredCommandInterface,
            string platformName)
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

            var classGenerators = GetClassGenerators();

            // we skip building the global namespace as gives an empty name
            foreach (var namespaceMember in globalNamespace.GetNamespaceMembers())
            {
                var nestedDeclarationSyntax = CheckNamespaceForUiTypes(
                    namespaceMember,
                    reportDiagnosticAction,
                    baseUiElement,
                    desiredBaseTypeIsInterface,
                    previouslyGeneratedClasses,
                    desiredCommandInterface,
                    platformName,
                    classGenerators);

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

        private void CheckTypeForUiType(INamedTypeSymbol namedTypeSymbol,
            Action<Diagnostic> reportDiagnosticAction,
            string baseUiElement,
            bool desiredBaseTypeIsInterface,
            IList<string> previouslyGeneratedClasses,
            string desiredCommandInterface,
            string platformName,
            Func<IClassGenerator>[] classGenerators,
            List<MemberDeclarationSyntax> memberDeclarationSyntaxes)
        {
            var fullName = namedTypeSymbol.GetFullName();

            try
            {
                // this is in a try catch block because roslyn sometimes throws a null ref exception.
                var accessibility = namedTypeSymbol.DeclaredAccessibility;
                if (accessibility != Accessibility.Public)
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            // ensure we inherit from our desired element.
            if (namedTypeSymbol.IsSealed ||
                namedTypeSymbol.IsStatic ||
                (!HasDesiredBaseType(
                     baseUiElement,
                     desiredBaseTypeIsInterface,
                     namedTypeSymbol) &&
                !fullName.Equals(baseUiElement, StringComparison.Ordinal)) ||
                previouslyGeneratedClasses.Any(pgc => pgc.Equals(fullName)))
            {
                return;
            }

            previouslyGeneratedClasses.Add(fullName);
            reportDiagnosticAction(ReportDiagnostics.HasDesiredBaseType(baseUiElement, namedTypeSymbol));

            foreach (var classGeneratorFactory in classGenerators)
            {
                var generator = classGeneratorFactory();
                var generatedClass = generator.GenerateClass(
                    namedTypeSymbol,
                    baseUiElement,
                    desiredCommandInterface,
                    platformName);

                memberDeclarationSyntaxes.Add(generatedClass);
            }
        }

        private NamespaceDeclarationSyntax CheckNamespaceForUiTypes(INamespaceSymbol namespaceSymbol,
            Action<Diagnostic> reportDiagnosticAction,
            string baseUiElement,
            bool desiredBaseTypeIsInterface,
            IList<string> previouslyGeneratedClasses,
            string desiredCommandInterface,
            string platformName,
            Func<IClassGenerator>[] classGenerators)
        {
            reportDiagnosticAction(ReportDiagnostics.StartingScanOfNamespace(namespaceSymbol));

            var namedTypeSymbols = namespaceSymbol.GetTypeMembers();

            var nestedMembers = new List<MemberDeclarationSyntax>(namedTypeSymbols.Length);

            foreach (var namedTypeSymbol in namedTypeSymbols)
            {
                CheckTypeForUiType(
                    namedTypeSymbol,
                    reportDiagnosticAction,
                    baseUiElement,
                    desiredBaseTypeIsInterface,
                    previouslyGeneratedClasses,
                    desiredCommandInterface,
                    platformName,
                    classGenerators,
                    nestedMembers);
            }

            var nestedSymbols = namespaceSymbol.GetNamespaceMembers();

            foreach (var nestedNamespaceSymbol in nestedSymbols)
            {
                var nestedNamespace = CheckNamespaceForUiTypes(
                    nestedNamespaceSymbol,
                    reportDiagnosticAction,
                    baseUiElement,
                    desiredBaseTypeIsInterface,
                    previouslyGeneratedClasses,
                    desiredCommandInterface,
                    platformName,
                    classGenerators);

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

        private bool HasDesiredBaseType(
            string desiredBaseType,
            bool desiredBaseTypeIsInterface,
            INamedTypeSymbol namedTypeSymbol)
        {
            var baseType = namedTypeSymbol;

            while (baseType != null)
            {
                var baseTypeFullName = baseType.GetFullName();
                if (desiredBaseTypeIsInterface)
                {
                    var interfaces = baseType.Interfaces;
                    if (interfaces != null && baseType.Interfaces.Any(i => i.GetFullName().Equals(desiredBaseType, StringComparison.Ordinal)))
                    {
                        return true;
                    }
                }
                else
                {
                    if (baseTypeFullName.Equals(desiredBaseType, StringComparison.Ordinal))
                    {
                        return true;
                    }
                }

                if (baseTypeFullName.Equals("global::System.Object", StringComparison.Ordinal))
                {
                    // we can drop out 1 iteration early
                    return false;
                }

                baseType = baseType.BaseType;
            }

            return false;
        }
    }
}
