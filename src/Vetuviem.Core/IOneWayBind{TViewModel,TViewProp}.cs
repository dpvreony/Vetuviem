// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
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
    /// <typeparam name="TVetuviemTargetViewModel">The type for the target ViewModel that Vetuviem will bind to.</typeparam>
    /// <typeparam name="TViewProp">The type for the View.</typeparam>
    public interface IOneWayBind<TVetuviemTargetViewModel, TViewProp>
        where TVetuviemTargetViewModel : class
    {
        /// <summary>
        /// Gets the binding to apply between the ViewModel and the View.
        /// </summary>
        Expression<Func<TVetuviemTargetViewModel, TViewProp?>> ViewModelBinding { get; }

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
            TVetuviemTargetViewModel viewModel,
            Expression<Func<TView, TViewProp>> viewBinding)
            where TView : class, IViewFor<TVetuviemTargetViewModel>;

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
            TVetuviemTargetViewModel viewModel,
            Expression<Func<TView, TViewProp>> viewBinding)
            where TView : class, IViewFor<TVetuviemTargetViewModel>;
    }
}
