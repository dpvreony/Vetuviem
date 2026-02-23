// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace Vetuviem.SourceGenerator.Features.Configuration
{
    /// <summary>
    /// Represents the selected UI framework for generation.
    /// </summary>
    public enum UiFramework
    {
        /// <summary>
        /// No UI framework selected.
        /// </summary>
        None,

        /// <summary>
        /// Avalonia UI framework.
        /// </summary>
        Avalonia,

        /// <summary>
        /// Blazor UI framework.
        /// </summary>
        Blazor,

        /// <summary>
        /// Maui UI framework.
        /// </summary>
        Maui,

        /// <summary>
        /// Winforms UI framework.
        /// </summary>
        Winforms,

        /// <summary>
        /// WinUI UI framework.
        /// </summary>
        WinUi,

        /// <summary>
        /// WPF UI framework.
        /// </summary>
        Wpf
    }
}
