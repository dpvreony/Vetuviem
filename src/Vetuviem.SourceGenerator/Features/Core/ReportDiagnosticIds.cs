// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace Vetuviem.SourceGenerator.Features.Core
{
    /// <summary>
    /// Diagnostic ID string constants.
    /// </summary>
    public static class ReportDiagnosticIds
    {
        /// <summary>
        /// Diagnostic ID for when the source generator is being started.
        /// </summary>
        public const string StartingSourceGenerator = "VET001";

        /// <summary>
        /// Diagnostic ID for when there in a mismatch in the search for the references of interest.
        /// </summary>
        public const string ReferencesOfInterestCountMismatch = "VET002";

        /// <summary>
        /// Diagnostic ID for when a Base UI element for a framework has been matched.
        /// </summary>
        public const string MatchedBaseUiElement = "VET003";

        /// <summary>
        /// Diagnostic ID for when an internal unhandled exception has occurred.
        /// </summary>
        public const string UnhandledException = "VET004";

        /// <summary>
        /// Diagnostic ID for when a type has a desired base type.
        /// </summary>
        public const string HasDesiredBaseType = "VET005";

        /// <summary>
        /// Diagnostic ID for when the scan of a namespace is starting.
        /// </summary>
        public const string StartingScanOfNamespace = "VET006";

        /// <summary>
        /// Diagnostic ID for when a metadata reference is not an assembly symbol.
        /// </summary>
        public const string MetadataReferenceNotAssemblySymbol = "VET007";

        /// <summary>
        /// Diagnostic ID for when the scan of an assembly is started.
        /// </summary>
        public const string StartingScanOfAssembly = "VET008";

        /// <summary>
        /// Diagnostic ID for when there is no assembly or module symbol.
        /// </summary>
        public const string NoAssemblyOrModuleSybmol = "VET009";

        /// <summary>
        /// Diagnostic ID for when there is no global namespace in an assembly or module.
        /// </summary>
        public const string NoGlobalNamespaceInAssemblyOrModule = "VET010";

        /// <summary>
        /// Diagnostic ID for when the desired base type symbol is not an interface or class.
        /// </summary>
        public const string DesiredBaseTypeSymbolNotInterfaceOrClass = "VET011";

        /// <summary>
        /// Diagnostic ID for when the scan of a type is starting.
        /// </summary>
        public const string StartingCheckOfType = "VET012";

        /// <summary>
        /// Diagnostic ID for when the desired base type symbol has not been found during a search of the framework.
        /// </summary>
        public const string FailedToFindDesiredBaseTypeSymbol = "VET013";

        /// <summary>
        /// Diagnostic ID for when more than 1 result has been found during the desired base type search.
        /// </summary>
        public const string DesiredBaseTypeSymbolSearchResultNotUnique = "VET014";

        /// <summary>
        /// Diagnostic ID for when the desired base type symbol has not been found on a type.
        /// </summary>
        public const string DesiredBaseTypeSymbolSearchNotNamedTypeSymbol = "VET015";
    }
}
