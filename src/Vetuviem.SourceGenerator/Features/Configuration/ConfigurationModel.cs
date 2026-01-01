using System.Collections.Generic;

namespace Vetuviem.SourceGenerator.Features.Configuration
{
    /// <summary>
    /// Configuration Model for the Source Generator.
    /// </summary>
    /// <param name="RootNamespace">The root name to apply if it's being overriden.</param>
    /// <param name="MakeClassesPublic">Whether to make classes public.</param>
    /// <param name="AssembliesArray">The assemblies to extend or override with, if any.</param>
    /// <param name="AssemblyMode">The action that will carried out with the specified assemblies.</param>
    /// <param name="BaseType">
    /// base type name only used if passing a custom set of assemblies to search for.
    /// allows for 3rd parties to use the generator and produce a custom namespace that inherits off the root, or custom namespace.
    /// </param>
    /// <param name="IncludeObsoleteItems">Whether to include obsolete items in the generation.</param>
    /// <param name="AllowExperimentalProperties">Whether to include properties marked with ExperimentalAttribute. If true, warnings will be suppressed.</param>
    /// <param name="LoggingImplementationMode">The logging implementation mode to use for generated code.</param>
    public sealed record ConfigurationModel(
        string? RootNamespace,
        bool MakeClassesPublic,
        IReadOnlyCollection<string>? AssembliesArray,
        AssemblyMode AssemblyMode,
        string? BaseType,
        bool IncludeObsoleteItems,
        bool AllowExperimentalProperties,
        LoggingImplementationMode LoggingImplementationMode);
}
