// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace Vetuviem.SourceGenerator.Features.Configuration
{
    /// <summary>
    /// Represents the UI platforms available for generation.
    /// </summary>
    public enum UiPlatform
    {
        /// <summary>
        /// No platform has been selected.
        /// </summary>
        None,

        /// <summary>
        /// Blazor Platform used for generation.
        /// </summary>
        Blazor,

        /// <summary>
        /// MAUI has been selected for generation.
        /// </summary>
        Maui,

        /// <summary>
        /// Winforms has been selected for generation.
        /// </summary>
        Winforms,

        /// <summary>
        /// WPF has been selected for generation.
        /// </summary>
        Wpf,

        /// <summary>
        /// Xamarin Forms has been selected for generation.
        /// </summary>
        XamForms,

        /// <summary>
        /// A custom platform has been selected for generation. This requires
        /// extra configuration and is in line with how version 0.9 operated
        /// where each platform had its own library with the source generation
        /// class in.
        /// </summary>
        Custom = int.MaxValue
    }
}
