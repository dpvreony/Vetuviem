// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Windows.Input;
using ReactiveUI;

namespace Vetuviem.Core
{
    /// <summary>
    /// Represents a command binding between a control and a viewmodel.
    /// </summary>
    /// <typeparam name="TVetuviemTargetViewModel">The type for the target viewmodel that Vetuviem will bind to.</typeparam>
    public sealed class CommandBinding<TVetuviemTargetViewModel> : ICommandBinding<TVetuviemTargetViewModel>
        where TVetuviemTargetViewModel : class
    {
        private readonly Expression<Func<TVetuviemTargetViewModel, ICommand?>> _viewModelBinding;
        private readonly string? _toEvent;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandBinding{TViewModel}"/> class.
        /// </summary>
        /// <param name="viewModelBinding">Expression for the View Model binding.</param>
        /// <param name="toEvent">If specified, bind to the specific event instead of the default.</param>
        public CommandBinding(
            Expression<Func<TVetuviemTargetViewModel, ICommand?>> viewModelBinding,
            string? toEvent = null)
        {
            _viewModelBinding = viewModelBinding;
            _toEvent = toEvent;
        }

        /// <inheritdoc/>
        public void ApplyBinding<TView, TViewProp>(
            Action<IDisposable> disposeAction,
            TView view,
            TVetuviemTargetViewModel viewModel,
            Expression<Func<TView, TViewProp>> viewBinding)
            where TView : class, IViewFor<TVetuviemTargetViewModel>
            where TViewProp : class
        {
            if (disposeAction == null)
            {
                throw new ArgumentNullException(nameof(disposeAction));
            }

            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            if (viewBinding == null)
            {
                throw new ArgumentNullException(nameof(viewBinding));
            }

            disposeAction(view.BindCommand(
                viewModel,
                _viewModelBinding,
                viewBinding,
                _toEvent));
        }

        /// <inheritdoc/>
        public void ApplyBinding<TView, TViewProp>(
            CompositeDisposable compositeDisposable,
            TView view,
            TVetuviemTargetViewModel viewModel,
            Expression<Func<TView, TViewProp>> viewBinding)
            where TView : class, IViewFor<TVetuviemTargetViewModel>
            where TViewProp : class
        {
            if (compositeDisposable == null)
            {
                throw new ArgumentNullException(nameof(compositeDisposable));
            }

            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            if (viewBinding == null)
            {
                throw new ArgumentNullException(nameof(viewBinding));
            }

            view.BindCommand(
                viewModel,
                _viewModelBinding,
                viewBinding,
                _toEvent).DisposeWith(compositeDisposable);
        }
    }
}
