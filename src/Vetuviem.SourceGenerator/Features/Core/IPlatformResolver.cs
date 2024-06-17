// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace Vetuviem.SourceGenerator.Features.Core
{
    /// <summary>
    /// Represents the logic used for driving type searching in a UI platform.
    /// </summary>
    public interface IPlatformResolver
    {
        /// <summary>
        /// Gets the assembly names used to resolve platform types.
        /// </summary>
        /// <returns>one or more assembly names.</returns>
        string[] GetAssemblyNames();

        /// <summary>
        /// Gets the fully qualified type that UI elements within the platform inherit from.
        /// </summary>
        /// <returns>Fully qualified type name.</returns>
        string GetBaseUiElement();

        /// <summary>
        /// Gets the fully qualified type that platform uses for binding controls to commands.
        /// </summary>
        /// <returns>Fully qualified type name, or null if no command implementation in the platform.</returns>
        string? GetCommandSourceInterface();

        /// <summary>
        /// Gets the fully qualified type that platform uses for commands.
        /// </summary>
        /// <returns>Fully qualified type name, or null if no command implementation in the platform.</returns>
        string? GetCommandInterface();
    }
}
