// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Disposables;
using ReactiveUI;

namespace Vetuviem.Core
{
    /// <summary>
    /// Represents a View to View Model Binding.
    /// </summary>
    /// <typeparam name="TView">The type for the view.</typeparam>
    /// <typeparam name="TViewModel">The type for the viewmodel.</typeparam>
    public interface IEnableViewToViewModelBindings<in TView, in TViewModel>
        where TView : class, IViewFor<TViewModel>
        where TViewModel : class, IReactiveObject
    {
        /// <summary>
        /// Apply control bindings between a View and ViewModel.
        /// </summary>
        /// <param name="disposeWithAction">The ReactiveUI Disposal Tracker. Used to discard binding registrations when the view is finished with them.</param>
        /// <param name="view">Instance of the view.</param>
        /// <param name="viewModel">Instance of the viewmodel.</param>
        void ApplyBindings(
            Action<IDisposable> disposeWithAction,
            TView view,
            TViewModel viewModel);

        /// <summary>
        /// Apply control bindings between a View and ViewModel.
        /// </summary>
        /// <param name="compositeDisposable">The Composite Disposable Tracker. Used to discard binding registrations when the view is finished with them.</param>
        /// <param name="view">Instance of the view.</param>
        /// <param name="viewModel">Instance of the viewmodel.</param>
        void ApplyBindings(
            CompositeDisposable compositeDisposable,
            TView view,
            TViewModel viewModel);
    }
}
