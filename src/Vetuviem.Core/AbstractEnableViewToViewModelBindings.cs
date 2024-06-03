// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using ReactiveUI;

namespace Vetuviem.Core
{
    /// <summary>
    /// Abstraction for View to View model binding. This stores a collection of control model bindings.
    /// It's intended as a single point for invoking the binding to be applied on a view.
    /// </summary>
    /// <typeparam name="TView">The type for the view.</typeparam>
    /// <typeparam name="TViewModel">The type for the viewmodel.</typeparam>
    public abstract class AbstractEnableViewToViewModelBindings<TView, TViewModel> : IEnableViewToViewModelBindings<TView, TViewModel>
        where TView : class, IViewFor<TViewModel>
        where TViewModel : class, IReactiveObject
    {
        /// <inheritdoc />
        public void ApplyBindings(
            Action<IDisposable> disposeWithAction,
            TView view,
            TViewModel viewModel)
        {
            var bindings = GetBindings();
            foreach (var viewBindingModel in bindings)
            {
                viewBindingModel.ApplyBindings(
                    view,
                    viewModel,
                    disposeWithAction);
            }
        }

        /// <inheritdoc />
        public void ApplyBindings(
            CompositeDisposable compositeDisposable,
            TView view,
            TViewModel viewModel)
        {
            var bindings = GetBindings();
            foreach (var viewBindingModel in bindings)
            {
                viewBindingModel.ApplyBindings(
                    view,
                    viewModel,
                    compositeDisposable);
            }
        }

        /// <summary>
        /// Gets the controls to be bound on the view.
        /// </summary>
        /// <returns>Collection of control to viewmodel bindings.</returns>
        protected abstract IEnumerable<IControlBindingModel<TView, TViewModel>> GetBindings();
    }
}
