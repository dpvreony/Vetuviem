// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace Vetuviem.SourceGenerator.Features.Configuration
{
    /// <summary>
    /// Represents the mode used for building platform assemblies.
    /// </summary>
    public enum AssemblyIncludeMode
    {
        /// <summary>
        /// No mode specified.
        /// </summary>
        None,

        /// <summary>
        /// Empty assembly mode. Use when another library already contains the default platform
        /// assemblies and you are extending with a specialist subset. For example you are
        /// generating for a specific ui control and extending your base set of generation.
        /// </summary>
        Empty,

        /// <summary>
        /// Default Platform assemblies. Used to include the default platform assemblies you
        /// can extend with additional assemblies, the specific reasons for using this are
        /// 1) you are building a base library and the downstream assembly is going to use the Empty Mode
        /// 2) you are building a single assembly for all controls.
        /// </summary>
        DefaultPlatform
    }
}
