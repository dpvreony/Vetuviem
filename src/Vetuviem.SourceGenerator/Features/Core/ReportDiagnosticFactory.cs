// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using Microsoft.CodeAnalysis;

namespace Vetuviem.SourceGenerator.Features.Core
{
    /// <summary>
    /// Helper class for producing reporting diagnostic events.
    /// </summary>
    public static class ReportDiagnosticFactory
    {
        public static Diagnostic HasDesiredBaseType(string desiredBaseType, INamedTypeSymbol namedTypeSymbol)
        {
            return InfoDiagnostic(
                ReportDiagnosticIds.HasDesiredBaseType,
                $"{namedTypeSymbol.GetFullName()} has desired base type {desiredBaseType}");
        }

        public static Diagnostic StartingSourceGenerator()
        {
            return InfoDiagnostic(
                ReportDiagnosticIds.StartingSourceGenerator,
                $"Starting Source Generator");
        }

        public static Diagnostic StartingScanOfAssembly(MetadataReference metadataReference)
        {
            return InfoDiagnostic(
                ReportDiagnosticIds.StartingScanOfAssembly,
                $"Starting Scan Of Namespace: {metadataReference.Display}");
        }

        public static Diagnostic StartingScanOfNamespace(INamespaceSymbol namespaceSymbol)
        {
            return InfoDiagnostic(
                ReportDiagnosticIds.StartingScanOfNamespace,
                $"Starting Scan Of Namespace: {namespaceSymbol}");
        }

        public static Diagnostic ReferencesOfInterestCountMismatch(int expected, int actual)
        {
            return ErrorDiagnostic(
                ReportDiagnosticIds.ReferencesOfInterestCountMismatch,
                $"The number of assemblies we want for this generator does not match the amount matched. Expected: {expected}, Actual: {actual}");
        }

        public static Diagnostic MatchedBaseUiElement(string desiredBaseType)
        {
            return InfoDiagnostic(
                ReportDiagnosticIds.MatchedBaseUiElement,
                $"Matched Base Ui Element: {desiredBaseType}");
        }

        public static Diagnostic UnhandledException(Exception exception)
        {
            return ErrorDiagnostic(
                ReportDiagnosticIds.UnhandledException,
                exception.ToString());
        }

        public static Diagnostic MetadataReferenceNotAssemblySymbol(MetadataReference metadataReference)
        {
            return ErrorDiagnostic(
                ReportDiagnosticIds.MetadataReferenceNotAssemblySymbol,
                $"Metadata Reference Not Assembly Symbol {metadataReference.Display}");
        }

        public static Diagnostic StartingCheckOfType(string fullName)
        {
            return InfoDiagnostic(
                ReportDiagnosticIds.StartingCheckOfType,
                $"Starting Check Of Type: {fullName}");
        }

        public static Diagnostic FailedToFindDesiredBaseTypeSymbol(string desiredTypeName)
        {
            return ErrorDiagnostic(
                ReportDiagnosticIds.FailedToFindDesiredBaseTypeSymbol,
                $"Failed To Find Desired Base Type Symbol: {desiredTypeName}");
        }

        public static Diagnostic DesiredBaseTypeSymbolSearchResultNotUnique(string desiredTypeName)
        {
            return ErrorDiagnostic(
                ReportDiagnosticIds.DesiredBaseTypeSymbolSearchResultNotUnique,
                $"Desired Base Type Symbol Search Result Not Unique: {desiredTypeName}");
        }

        public static Diagnostic DesiredBaseTypeSymbolSearchNotNamedTypeSymbol(string desiredTypeName)
        {
            return ErrorDiagnostic(
                ReportDiagnosticIds.DesiredBaseTypeSymbolSearchNotNamedTypeSymbol,
                $"Desired Base Type Symbol Search Not Named Type Symbol: {desiredTypeName}");
        }

        public static Diagnostic DesiredBaseTypeSymbolNotInterfaceOrClass(string desiredTypeName)
        {
            return ErrorDiagnostic(
                ReportDiagnosticIds.DesiredBaseTypeSymbolNotInterfaceOrClass,
                $"Desired Base Type Symbol Search Not Named Type Symbol: {desiredTypeName}");
        }

        public static Diagnostic NoAssemblyOrModuleSybmol(MetadataReference metadataReference)
        {
            return ErrorDiagnostic(
                ReportDiagnosticIds.NoAssemblyOrModuleSybmol,
                $"No Assembly or Module Symbol: {metadataReference.Display}");
        }

        public static Diagnostic NoGlobalNamespaceInAssemblyOrModule(MetadataReference metadataReference)
        {
            return ErrorDiagnostic(
                ReportDiagnosticIds.NoGlobalNamespaceInAssemblyOrModule,
                $"No global namespace in Assembly or Module Symbol: {metadataReference.Display}");
        }

        private static Diagnostic InfoDiagnostic(
            string id,
            string message)
        {
            return GetDiagnotic(
                id,
                message,
                DiagnosticSeverity.Info,
                1);
        }

        private static Diagnostic ErrorDiagnostic(
            string id,
            string message)
        {
            return GetDiagnotic(
                id,
                message,
                DiagnosticSeverity.Error,
                0);
        }

        private static Diagnostic GetDiagnotic(
            string id,
            string message,
            DiagnosticSeverity severity,
            int warningLevel)
        {
            return Diagnostic.Create(
                id,
                "Vetuviem Generation",
                message,
                severity,
                severity,
                true,
                warningLevel,
                "Model Generation");
        }
    }
}
