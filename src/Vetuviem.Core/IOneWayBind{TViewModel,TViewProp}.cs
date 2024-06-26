﻿// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using ReactiveUI;

namespace Vetuviem.Core
{
    /// <summary>
    /// Represents a one way View and ViewModel binding.
    /// </summary>
    /// <typeparam name="TViewModel">The type for the ViewModel.</typeparam>
    /// <typeparam name="TViewProp">The type for the View.</typeparam>
    public interface IOneWayBind<TViewModel, TViewProp>
        where TViewModel : class
    {
        /// <summary>
        /// Gets the binding to apply between the ViewModel and the View.
        /// </summary>
        Expression<Func<TViewModel, TViewProp?>> ViewModelBinding { get; }

        /// <summary>
        /// Applies a View to View Model Binding.
        /// </summary>
        /// <typeparam name="TView">The type for the view.</typeparam>
        /// <param name="d">The disposable action registration. Used to clean up when bindings fall out of scope.</param>
        /// <param name="view">The instance of the View to bind.</param>
        /// <param name="viewModel">The instance of the ViewModel to Bind.</param>
        /// <param name="viewBinding">Expression of the View Property to Bind to.</param>
        void ApplyBinding<TView>(
            Action<IDisposable> d,
            TView view,
            TViewModel viewModel,
            Expression<Func<TView, TViewProp>> viewBinding)
            where TView : class, IViewFor<TViewModel>;

        /// <summary>
        /// Applies a View to View Model Binding.
        /// </summary>
        /// <typeparam name="TView">The type for the view.</typeparam>
        /// <param name="compositeDisposable">The disposable action registration. Used to clean up when bindings fall out of scope.</param>
        /// <param name="view">The instance of the View to bind.</param>
        /// <param name="viewModel">The instance of the ViewModel to Bind.</param>
        /// <param name="viewBinding">Expression of the View Property to Bind to.</param>
        void ApplyBinding<TView>(
            CompositeDisposable compositeDisposable,
            TView view,
            TViewModel viewModel,
            Expression<Func<TView, TViewProp>> viewBinding)
            where TView : class, IViewFor<TViewModel>;
    }
}
