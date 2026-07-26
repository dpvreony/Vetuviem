// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using ReactiveUI;
using ReactiveUI.Primitives.Concurrency;
using ReactiveUI.Primitives.Disposables;

namespace Vetuviem.Core
{
    /// <summary>
    /// Represents a View to View Model Binding.
    /// </summary>
    /// <typeparam name="TView">The type for the view.</typeparam>
    /// <typeparam name="TVetuviemTargetViewModel">The type for the target viewmodel that Vetuviem will bind to.</typeparam>
    public interface IEnableViewToViewModelBindings<in TView, in TVetuviemTargetViewModel>
        where TView : class, IViewFor<TVetuviemTargetViewModel>
        where TVetuviemTargetViewModel : class, IReactiveObject
    {
        /// <summary>
        /// Apply control bindings between a View and ViewModel.
        /// </summary>
        /// <param name="disposeWithAction">The ReactiveUI Disposal Tracker. Used to discard binding registrations when the view is finished with them.</param>
        /// <param name="view">Instance of the view.</param>
        /// <param name="viewModel">Instance of the viewmodel.</param>
        /// <param name="sequencer">Sequencer for subscriptions.</param>
        void ApplyBindings(
            Action<IDisposable> disposeWithAction,
            TView view,
            TVetuviemTargetViewModel viewModel,
            ISequencer? sequencer = null);

        /// <summary>
        /// Apply control bindings between a View and ViewModel.
        /// </summary>
        /// <param name="multipleDisposable">The Composite Disposable Tracker. Used to discard binding registrations when the view is finished with them.</param>
        /// <param name="view">Instance of the view.</param>
        /// <param name="viewModel">Instance of the viewmodel.</param>
        /// <param name="sequencer">Sequencer for subscriptions.</param>
        void ApplyBindings(
            MultipleDisposable multipleDisposable,
            TView view,
            TVetuviemTargetViewModel viewModel,
            ISequencer? sequencer = null);
    }
}
