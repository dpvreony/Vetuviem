// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
        /// <returns>Class Declaration Syntax Node.</returns>
        ClassDeclarationSyntax GenerateClass(
            INamedTypeSymbol namedTypeSymbol,
            string baseUiElement,
            string? desiredCommandInterface,
            string platformName,
            string rootNamespace,
            bool makeClassesPublic);
    }
}
