namespace Vetuviem.SourceGenerator.Features.Core
{
    public static class ReportDiagnosticIds
    {
        public static string StartingSourceGenerator => "VET-001";
        public static string ReferencesOfInterestCountMismatch => "VET-002";
        public static string MatchedBaseUiElement => "VET-003";
        public static string UnhandledException => "VET-004";
        public static string HasDesiredBaseType => "VET-005";
        public static string StartingScanOfNamespace => "VET-006";
        public static string MetadataReferenceNotAssemblySymbol => "VET-007";
        public static string StartingScanOfAssembly => "VET-008";
        public static string NoAssemblyOrModuleSybmol => "VET-009";
        public static string NoGlobalNamespaceInAssemblyOrModule => "VET-010";
        public static string DesiredBaseTypeSymbolNotInterfaceOrClass => "VET-011";
        public static string StartingCheckOfType => "VET-012";
        public static string FailedToFindDesiredBaseTypeSymbol => "VET-013";
        public static string DesiredBaseTypeSymbolSearchResultNotUnique => "VET-014";
        public static string DesiredBaseTypeSymbolSearchNotNamedTypeSymbol => "VET-015";
    }
}
