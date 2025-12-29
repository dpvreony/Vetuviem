// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace Vetuviem.SourceGenerator.Features.Configuration
{
    /// <summary>
    /// Represents the logging implementation mode for the source generator.
    /// </summary>
    public enum LoggingImplementationMode
    {
        /// <summary>
        /// No logging implementation will be generated.
        /// </summary>
        None,

        /// <summary>
        /// Splat logging implementation will be generated.
        /// </summary>
        Splat,

        /// <summary>
        /// Microsoft.Extensions.Logging implementation will be generated.
        /// </summary>
        MicrosoftExtensionsLogging
    }
}
