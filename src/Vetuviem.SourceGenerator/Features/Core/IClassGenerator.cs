// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Vetuviem.SourceGenerator.Features.Configuration;

namespace Vetuviem.SourceGenerator.Features.Core
{
    /// <summary>
    /// Represents a generator for producing a class level member based upon another named type.
    /// </summary>
    public interface IClassGenerator
    {
        /// <summary>
        /// Generates the Roslyn model for a class based upon an input Named Type Symbol.
        /// </summary>
        /// <param name="namedTypeSymbol">The named type symbol to generate the class around.</param>
        /// <param name="baseUiElement">The fully qualified name for the Base UI element for the platform.</param>
        /// <param name="desiredCommandInterface">The fully qualified name for the command interface, if the platform supports commands.</param>
        /// <param name="platformName">The name of the Platform code is being generated for.</param>
        /// <param name="rootNamespace">The root namespace to place the binding classes inside.</param>
        /// <param name="makeClassesPublic">A flag indicating whether to expose the generated binding classes as public rather than internal. Set this to true if you're created a reusable library file.</param>
        /// <param name="includeObsoleteItems">Whether to include obsolete items in the generated code.</param>
        /// <param name="platformCommandType">The platform-specific command type.</param>
        /// <param name="allowExperimentalProperties">Whether to include properties marked with ExperimentalAttribute. If true, warnings will be suppressed.</param>
        /// <param name="loggingImplementationMode">The logging implementation mode to use for generated code.</param>
        /// <returns>Class Declaration Syntax Node.</returns>
        ClassDeclarationSyntax GenerateClass(
            INamedTypeSymbol namedTypeSymbol,
            string baseUiElement,
            string? desiredCommandInterface,
            string platformName,
            string rootNamespace,
            bool makeClassesPublic,
            bool includeObsoleteItems,
            string? platformCommandType,
            bool allowExperimentalProperties,
            LoggingImplementationMode loggingImplementationMode);
    }
}
