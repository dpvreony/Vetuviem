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
        /// <summary>
        /// Report for when a named type has the desired base type for a UI platform.
        /// </summary>
        /// <param name="desiredBaseType">The desired base type for the UI platform.</param>
        /// <param name="namedTypeSymbol">Named Type Symbol.</param>
        /// <returns>Diagnostic Instance.</returns>
        public static Diagnostic HasDesiredBaseType(string desiredBaseType, INamedTypeSymbol namedTypeSymbol)
        {
            return InfoDiagnostic(
                ReportDiagnosticIds.HasDesiredBaseType,
                $"{namedTypeSymbol.GetFullName()} has desired base type {desiredBaseType}");
        }

        /// <summary>
        /// Report for when a source generator is starting.
        /// </summary>
        /// <returns>Diagnostic Instance.</returns>
        public static Diagnostic StartingSourceGenerator()
        {
            return InfoDiagnostic(
                ReportDiagnosticIds.StartingSourceGenerator,
                $"Starting Source Generator");
        }

        /// <summary>
        /// Report for when the scan of an assembly is starting.
        /// </summary>
        /// <param name="metadataReference">Metadata reference for an assembly.</param>
        /// <returns>Diagnostic Instance.</returns>
        public static Diagnostic StartingScanOfAssembly(MetadataReference metadataReference)
        {
            return InfoDiagnostic(
                ReportDiagnosticIds.StartingScanOfAssembly,
                $"Starting Scan Of Assembly: {metadataReference.Display}");
        }

        /// <summary>
        /// Report for when the scan of a namespace is starting.
        /// </summary>
        /// <param name="namespaceSymbol">Symbol reference for a namespace.</param>
        /// <returns>Diagnostic Instance.</returns>
        public static Diagnostic StartingScanOfNamespace(INamespaceSymbol namespaceSymbol)
        {
            return InfoDiagnostic(
                ReportDiagnosticIds.StartingScanOfNamespace,
                $"Starting Scan Of Namespace: {namespaceSymbol}");
        }

        /// <summary>
        /// Report for when after assembly reference scanning, there is a mismatch in the number of assemblies found versus what was expected.
        /// </summary>
        /// <param name="expected">Expected number of assemblies.</param>
        /// <param name="actual">The actual number of assemblies.</param>
        /// <returns>Diagnostic Instance.</returns>
        public static Diagnostic ReferencesOfInterestCountMismatch(int expected, int actual)
        {
            return ErrorDiagnostic(
                ReportDiagnosticIds.ReferencesOfInterestCountMismatch,
                $"The number of assemblies we want for this generator does not match the amount matched. Expected: {expected}, Actual: {actual}");
        }

        /// <summary>
        /// Report for when an unhandled exception has occurred.
        /// </summary>
        /// <param name="exception">The exception that occurred.</param>
        /// <returns>Diagnostic Instance.</returns>
        public static Diagnostic UnhandledException(Exception exception)
        {
            return ErrorDiagnostic(
                ReportDiagnosticIds.UnhandledException,
                exception.ToString());
        }

        /// <summary>
        /// Report for when the desired base type of a UI platform has not been found during a scan of the reference assemblies.
        /// </summary>
        /// <param name="desiredTypeName">Fully qualified name of the desired type.</param>
        /// <returns>Diagnostic Instance.</returns>
        public static Diagnostic FailedToFindDesiredBaseTypeSymbol(string desiredTypeName)
        {
            return ErrorDiagnostic(
                ReportDiagnosticIds.FailedToFindDesiredBaseTypeSymbol,
                $"Failed To Find Desired Base Type Symbol: {desiredTypeName}");
        }

        /// <summary>
        /// Report for when the desired base type has been found, but is not a class or interface.
        /// </summary>
        /// <param name="desiredTypeName">Fully qualified name of the desired type.</param>
        /// <returns>Diagnostic Instance.</returns>
        public static Diagnostic DesiredBaseTypeSymbolNotInterfaceOrClass(string desiredTypeName)
        {
            return ErrorDiagnostic(
                ReportDiagnosticIds.DesiredBaseTypeSymbolNotInterfaceOrClass,
                $"Desired Base Type Symbol Search Not Named Type Symbol: {desiredTypeName}");
        }

        /// <summary>
        /// Report for when a metadata reference isn't for an assembly or module.
        /// </summary>
        /// <param name="metadataReference">Metadata reference for what should have been an assembly or module.</param>
        /// <returns>Diagnostic Instance.</returns>
        public static Diagnostic NoAssemblyOrModuleSybmol(MetadataReference metadataReference)
        {
            return ErrorDiagnostic(
                ReportDiagnosticIds.NoAssemblyOrModuleSybmol,
                $"No Assembly or Module Symbol: {metadataReference.Display}");
        }

        /// <summary>
        /// Report for when an assembly\ module doesn't contain a global namespace.
        /// </summary>
        /// <param name="metadataReference">Metadata reference for an assembly or module.</param>
        /// <returns>Diagnostic Instance.</returns>
        public static Diagnostic NoGlobalNamespaceInAssemblyOrModule(MetadataReference metadataReference)
        {
            return ErrorDiagnostic(
                ReportDiagnosticIds.NoGlobalNamespaceInAssemblyOrModule,
                $"No global namespace in Assembly or Module Symbol: {metadataReference.Display}");
        }

        public static Diagnostic IsObsoleteType(INamedTypeSymbol namedTypeSymbol)
        {
            return InfoDiagnostic(
                ReportDiagnosticIds.IsObsoleteType,
                $"{namedTypeSymbol.GetFullName()} is obsolete");
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
