// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vetuviem.SourceGenerator.Features.Core
{
    /// <summary>
    /// Abstraction for a Code Generation Processor.
    /// </summary>
    public abstract class AbstractGeneratorProcessor
    {
        /// <summary>
        /// Generates a Namespace Declaration.
        /// </summary>
        /// <param name="namespaceDeclaration">Roslyn Namespace Declaration to extend.</param>
        /// <param name="assembliesOfInterest">Collection of assemblies to generate code around.</param>
        /// <param name="compilation">Compilation Unit.</param>
        /// <param name="reportDiagnosticAction">Action for reporting a diagnostic to the build pipeline.</param>
        /// <param name="desiredBaseType">Fully qualified name of the UI platform base type for a control.</param>
        /// <param name="desiredBaseTypeIsInterface">Flag indicating whether the desiredBaseType is an interface.</param>
        /// <param name="desiredCommandInterface">Fully qualified name for the desired command interface, if any.</param>
        /// <param name="platformName">Name of the UI Platform.</param>
        /// <param name="rootNamespace">The root namespace to place the binding classes inside.</param>
        /// <param name="makeClassesPublic">A flag indicating whether to expose the generated binding classes as public rather than internal. Set this to true if you're created a reusable library file.</param>
        /// <param name="includeObsoleteItems">Whether to include obsolete items in the generated code.</param>
        /// <returns>Namespace declaration containing generated code.</returns>
        public NamespaceDeclarationSyntax GenerateNamespaceDeclaration(NamespaceDeclarationSyntax namespaceDeclaration,
            MetadataReference[] assembliesOfInterest,
            Compilation compilation,
            Action<Diagnostic> reportDiagnosticAction,
            string desiredBaseType,
            bool desiredBaseTypeIsInterface,
            string? desiredCommandInterface,
            string platformName,
            string rootNamespace,
            bool makeClassesPublic,
            bool includeObsoleteItems)
        {
            if (namespaceDeclaration == null)
            {
                throw new ArgumentNullException(nameof(namespaceDeclaration));
            }

            if (assembliesOfInterest == null)
            {
                throw new ArgumentNullException(nameof(assembliesOfInterest));
            }

            if (compilation == null)
            {
                throw new ArgumentNullException(nameof(compilation));
            }

            if (reportDiagnosticAction == null)
            {
                throw new ArgumentNullException(nameof(reportDiagnosticAction));
            }

            if (string.IsNullOrWhiteSpace(desiredBaseType))
            {
                throw new ArgumentNullException(nameof(desiredBaseType));
            }

            if (string.IsNullOrWhiteSpace(platformName))
            {
                throw new ArgumentNullException(nameof(platformName));
            }

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
                    platformName,
                    rootNamespace,
                    makeClassesPublic,
                    includeObsoleteItems);
            }

            return namespaceDeclaration;
        }

        /// <summary>
        /// Function for getting a collection of class generators.
        /// </summary>
        /// <returns>Function to invoke.</returns>
        protected abstract Func<IClassGenerator>[] GetClassGenerators();

        private static INamespaceSymbol? GetGlobalNamespace(ISymbol assemblyOrModuleSymbol)
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

        private static void CheckTypeForUiType(INamedTypeSymbol namedTypeSymbol,
            Action<Diagnostic> reportDiagnosticAction,
            string baseUiElement,
            bool desiredBaseTypeIsInterface,
            IList<string> previouslyGeneratedClasses,
            string? desiredCommandInterface,
            string platformName,
            Func<IClassGenerator>[] classGenerators,
            List<MemberDeclarationSyntax> memberDeclarationSyntaxes,
            string rootNamespace,
            bool makeClassesPublic,
            bool includeObsoleteItems)
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
#pragma warning disable CA1031 // Do not catch general exception types
            catch
#pragma warning restore CA1031 // Do not catch general exception types
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
                previouslyGeneratedClasses.Any(pgc => pgc.Equals(fullName, StringComparison.Ordinal)))
            {
                return;
            }

            previouslyGeneratedClasses.Add(fullName);
            reportDiagnosticAction(ReportDiagnosticFactory.HasDesiredBaseType(baseUiElement, namedTypeSymbol));

            // check for obsolete attribute
            var attributes = namedTypeSymbol.GetAttributes();
            if (!includeObsoleteItems && attributes.Any(a => a.AttributeClass?.Name.Equals("ObsoleteAttribute", StringComparison.Ordinal) == true))
            {
                reportDiagnosticAction(ReportDiagnosticFactory.IsObsoleteType(namedTypeSymbol));
                return;
            }

            foreach (var classGeneratorFactory in classGenerators)
            {
                var generator = classGeneratorFactory();
                var generatedClass = generator.GenerateClass(
                    namedTypeSymbol,
                    baseUiElement,
                    desiredCommandInterface,
                    platformName,
                    rootNamespace,
                    makeClassesPublic);

                memberDeclarationSyntaxes.Add(generatedClass);
            }
        }

        private static bool HasDesiredBaseType(
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

        private NamespaceDeclarationSyntax CheckAssemblyForUiTypes(NamespaceDeclarationSyntax namespaceDeclaration,
            MetadataReference metadataReference,
            Compilation compilation,
            Action<Diagnostic> reportDiagnosticAction,
            string baseUiElement,
            bool desiredBaseTypeIsInterface,
            IList<string> previouslyGeneratedClasses,
            string? desiredCommandInterface,
            string platformName,
            string rootNamespace,
            bool makeClassesPublic,
            bool includeObsoleteItems)
        {
            reportDiagnosticAction(ReportDiagnosticFactory.StartingScanOfAssembly(metadataReference));

            var assemblyOrModuleSymbol = compilation.GetAssemblyOrModuleSymbol(metadataReference);

            if (assemblyOrModuleSymbol == null)
            {
                reportDiagnosticAction(ReportDiagnosticFactory.NoAssemblyOrModuleSybmol(metadataReference));
                return namespaceDeclaration;
            }

            var globalNamespace = GetGlobalNamespace(assemblyOrModuleSymbol);
            if (globalNamespace == null)
            {
                reportDiagnosticAction(ReportDiagnosticFactory.NoGlobalNamespaceInAssemblyOrModule(metadataReference));
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
                    classGenerators,
                    rootNamespace,
                    makeClassesPublic,
                    includeObsoleteItems);

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

        private NamespaceDeclarationSyntax? CheckNamespaceForUiTypes(INamespaceSymbol namespaceSymbol,
            Action<Diagnostic> reportDiagnosticAction,
            string baseUiElement,
            bool desiredBaseTypeIsInterface,
            IList<string> previouslyGeneratedClasses,
            string? desiredCommandInterface,
            string platformName,
            Func<IClassGenerator>[] classGenerators,
            string rootNamespace,
            bool makeClassesPublic,
            bool includeObsoleteItems)
        {
            reportDiagnosticAction(ReportDiagnosticFactory.StartingScanOfNamespace(namespaceSymbol));

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
                    nestedMembers,
                    rootNamespace,
                    makeClassesPublic,
                    includeObsoleteItems);
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
                    classGenerators,
                    rootNamespace,
                    makeClassesPublic,
                    includeObsoleteItems);

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
    }
}
