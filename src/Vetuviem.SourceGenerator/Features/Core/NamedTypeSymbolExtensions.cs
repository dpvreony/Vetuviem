// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using Microsoft.CodeAnalysis;

namespace Vetuviem.SourceGenerator.Features.Core
{
    /// <summary>
    /// Extension methods for working with Named Type Symbols.
    /// </summary>
    public static class NamedTypeSymbolExtensions
    {
        /// <summary>
        /// Gets the full name for a named type symbol.
        /// </summary>
        /// <param name="namedTypeSymbol">The named type symbol.</param>
        /// <returns>The full name.</returns>
        public static string GetFullName(this INamedTypeSymbol namedTypeSymbol)
        {
            if (namedTypeSymbol == null)
            {
                throw new ArgumentNullException(nameof(namedTypeSymbol));
            }

            return namedTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        }
    }
}
