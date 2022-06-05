// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Vetuviem.SourceGenerator.Features.Configuration
{
    /// <summary>
    /// Represents the configuration loaded from a project file.
    /// </summary>
    public sealed class GenerationConfigurationModel
    {
        /// <summary>
        /// Gets or sets the UI Platform to generate code for.
        /// </summary>
        public UiPlatform UiPlatform { get; set; }

        /// <summary>
        /// Gets or sets the Custom Platform settings, only used if the <see cref="UiPlatform"/> is set to Custom.
        /// </summary>
        public PlatformSettingsModel? CustomPlatformSettings { get; set; }

        /// <summary>
        /// Gets or sets the assembly include mode.
        /// </summary>
        public AssemblyIncludeMode AssemblyIncludeMode { get; set; }

        /// <summary>
        /// Gets or sets the list of additional assemblies to scan for the platform.
        /// </summary>
        public IList<string>? AdditionalAssemblies { get; set; }
    }
}
