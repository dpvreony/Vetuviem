// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Disposables;
using ReactiveUI;

namespace Vetuviem.Core
{
    /// <summary>
    /// Represents a control binding model.
    /// </summary>
    /// <typeparam name="TView">The type for the view.</typeparam>
    /// <typeparam name="TVetuviemTargetViewModel">The type for the target viewmodel that Vetuviem will bind to.</typeparam>
    public interface IControlBindingModel<in TView, in TVetuviemTargetViewModel>
        where TView : class, IViewFor<TVetuviemTargetViewModel>
        where TVetuviemTargetViewModel : class, IReactiveObject
    {
        /// <summary>
        /// Applies the binding between the view and the view model.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="viewModel">The viewmodel.</param>
        /// <param name="disposeAction">The action to register disposals against.</param>
        void ApplyBindings(TView view, TVetuviemTargetViewModel viewModel, Action<IDisposable> disposeAction);

        /// <summary>
        /// Applies the binding between the view and the view model.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="viewModel">The viewmodel.</param>
        /// <param name="compositeDisposable">The disposable container to register disposals against.</param>
        void ApplyBindings(TView view, TVetuviemTargetViewModel viewModel, CompositeDisposable compositeDisposable);
    }
}
