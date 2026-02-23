using System;
using System.Linq;
using Microsoft.CodeAnalysis.Diagnostics;
using Vetuviem.SourceGenerator.Features.Core;

namespace Vetuviem.SourceGenerator.Features.Configuration
{
    public static class ConfigurationFactory
    {
        public static ConfigurationModel Create(AnalyzerConfigOptionsProvider analyzerConfigOptionsProvider)
        {
            var globalOptions = analyzerConfigOptionsProvider.GlobalOptions;
            globalOptions.TryGetBuildPropertyValue(
                "Vetuviem_Root_Namespace",
                out var rootNamespace);

            globalOptions.TryGetBuildPropertyValue(
                "Vetuviem_Make_Classes_Public",
                out var makeClassesPublicAsString);
            bool.TryParse(
                makeClassesPublicAsString,
                out var makeClassesPublic);

            globalOptions.TryGetBuildPropertyValue(
                "Vetuviem_Assemblies",
                out var assemblies);
            var assembliesArray = assemblies?.Split(
                    [','],
                    StringSplitOptions.RemoveEmptyEntries)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToArray();

            globalOptions.TryGetBuildPropertyValue(
                "Vetuviem_Assembly_Mode",
                out var assemblyModeAsString);

            var assemblyMode = GetAssemblyMode(assemblyModeAsString);

            // base type name only used if passing a custom set of assemblies to search for.
            // allows for 3rd parties to use the generator and produce a custom namespace that inherits off the root, or custom namespace.
            globalOptions.TryGetBuildPropertyValue(
                "Vetuviem_Base_Namespace",
                out var baseType);

            globalOptions.TryGetBuildPropertyValue(
                "Vetuviem_Include_Obsolete_Items",
                out var includeObsoleteItemsAsString);
            bool.TryParse(
                includeObsoleteItemsAsString,
                out var includeObsoleteItems);

            globalOptions.TryGetBuildPropertyValue(
                "Vetuviem_Allow_Experimental_Properties",
                out var allowExperimentalPropertiesAsString);
            bool.TryParse(
                allowExperimentalPropertiesAsString,
                out var allowExperimentalProperties);

            globalOptions.TryGetBuildPropertyValue(
                "Vetuviem_Logging_Implementation_Mode",
                out var loggingImplementationModeAsString);
            var loggingImplementationMode = GetLoggingImplementationMode(loggingImplementationModeAsString);

            globalOptions.TryGetBuildPropertyValue(
                "Vetuviem_UI_Framework",
                out var uiFrameworkAsString);

            var uiFramework = GetUiFramework(uiFrameworkAsString);


            return new ConfigurationModel(
                rootNamespace,
                makeClassesPublic,
                assembliesArray,
                assemblyMode,
                baseType,
                includeObsoleteItems,
                allowExperimentalProperties,
                loggingImplementationMode,
                uiFramework);
        }

        private static UiFramework GetUiFramework(string? uiFrameworkAsString)
        {
            if (string.IsNullOrWhiteSpace(uiFrameworkAsString))
            {
                return UiFramework.None;
            }

            if (Enum.TryParse<UiFramework>(
                    uiFrameworkAsString,
                    true,
                    out var uiFramework))
            {
                return uiFramework;
            }

            throw new InvalidOperationException($"Invalid UI Framework: {uiFrameworkAsString}");
        }

        private static LoggingImplementationMode GetLoggingImplementationMode(string? loggingImplementationModeAsString)
        {
            if (string.IsNullOrWhiteSpace(loggingImplementationModeAsString))
            {
                return LoggingImplementationMode.SplatViaServiceLocator;
            }

            if (Enum.TryParse<LoggingImplementationMode>(
                    loggingImplementationModeAsString,
                    out var loggingImplementationMode))
            {
                return loggingImplementationMode;
            }

            throw new InvalidOperationException($"Invalid logging implementation mode: {loggingImplementationModeAsString}");
        }

        private static AssemblyMode GetAssemblyMode(string? assemblyModeAsString)
        {
            if (string.IsNullOrWhiteSpace(assemblyModeAsString))
            {
                return AssemblyMode.Replace;
            }

            if (Enum.TryParse<AssemblyMode>(
                    assemblyModeAsString,
                    out var assemblyMode))
            {
                return assemblyMode;
            }

            throw new InvalidOperationException("Invalid assembly mode.");
        }
    }
}
