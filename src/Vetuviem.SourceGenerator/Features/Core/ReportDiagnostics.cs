using System;
using Microsoft.CodeAnalysis;

namespace Vetuviem.SourceGenerator.Features.Core
{
    public static class ReportDiagnostics
    {
        public static Diagnostic StartingSourceGenerator()
        {
            return InfoDiagnostic(
                ReportDiagnosticIds.StartingSourceGenerator,
                $"Starting Source Generator");
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
