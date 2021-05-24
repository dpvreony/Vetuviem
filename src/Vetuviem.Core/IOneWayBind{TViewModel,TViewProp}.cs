// Copyright (c) 2020 DHGMS Solutions and Contributors. All rights reserved.
// DHGMS Solutions and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;

namespace Vetuviem.Core
{
    /// <summary>
    /// Represents a one way View and ViewModel binding.
    /// </summary>
    /// <typeparam name="TViewModel">The type for the ViewModel.</typeparam>
    /// <typeparam name="TViewProp">The type for the View.</typeparam>
    public interface IOneWayBind<TViewModel, TViewProp>
    {
        /// <summary>
        /// Gets the binding to apply between the ViewModel and the View.
        /// </summary>
        Expression<Func<TViewModel, TViewProp>> ViewModelBinding { get; }
    }
}
