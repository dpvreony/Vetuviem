using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
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

            return new ConfigurationModel(
                rootNamespace,
                makeClassesPublic,
                assembliesArray,
                assemblyMode,
                baseType,
                includeObsoleteItems);
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
