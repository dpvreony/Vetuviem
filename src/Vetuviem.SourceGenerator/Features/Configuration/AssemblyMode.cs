// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace Vetuviem.SourceGenerator.Features.Configuration
{
    /// <summary>
    /// The operation to carry out if assemblies are specified in the config.
    /// </summary>
    public enum AssemblyMode
    {
        /// <summary>
        /// None, this is a language default to pickup invalid values.
        /// </summary>
        None,

        /// <summary>
        /// Replace the default platform assemblies with the specified assemblies.
        /// </summary>
        Replace,

        /// <summary>
        /// Extend the default platform assemblies with the specified assemblies.
        /// </summary>
        Extend
    }
}
