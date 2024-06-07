// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Windows.Input;
using ReactiveUI;

namespace Vetuviem.Core
{
    /// <summary>
    /// Represents a command binding between a control and a viewmodel.
    /// </summary>
    /// <typeparam name="TViewModel">The type for the viewmodel.</typeparam>
    public class CommandBinding<TViewModel> : CommandBinding<TViewModel, Unit, Unit>
        where TViewModel : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandBinding{TViewModel}"/> class.
        /// </summary>
        /// <param name="viewModelBinding">Expression for the View Model binding.</param>
        /// <param name="toEvent">If specified, bind to the specific event instead of the default.</param>
        public CommandBinding(
            Expression<Func<TViewModel, ReactiveCommand<Unit, Unit>?>> viewModelBinding,
            string? toEvent = null)
            : base(
                viewModelBinding,
                toEvent)
        {
        }
    }

    /// <summary>
    /// Represents a command binding between a control and a viewmodel.
    /// </summary>
    /// <typeparam name="TViewModel">The type for the viewmodel.</typeparam>
    /// <typeparam name="TResult">
    /// The type of the values that are the result of command execution.
    /// </typeparam>
    public class CommandBinding<TViewModel, TResult> : CommandBinding<TViewModel, Unit, TResult>
        where TViewModel : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandBinding{TViewModel, TResult}"/> class.
        /// </summary>
        /// <param name="viewModelBinding">Expression for the View Model binding.</param>
        /// <param name="toEvent">If specified, bind to the specific event instead of the default.</param>
        public CommandBinding(
            Expression<Func<TViewModel, ReactiveCommand<Unit, TResult>?>> viewModelBinding,
            string? toEvent = null)
            : base(
                viewModelBinding,
                toEvent)
        {
        }
    }

    /// <summary>
    /// Represents a command binding between a control and a viewmodel.
    /// </summary>
    /// <typeparam name="TViewModel">The type for the viewmodel.</typeparam>
    /// <typeparam name="TParam">
    /// The type of parameter values passed in during command execution.
    /// </typeparam>
    /// <typeparam name="TResult">
    /// The type of the values that are the result of command execution.
    /// </typeparam>
    public class CommandBinding<TViewModel, TParam, TResult> : ICommandBinding<TViewModel, ICommand>
        where TViewModel : class
    {
        private readonly Expression<Func<TViewModel, ReactiveCommand<TParam, TResult>?>> _viewModelBinding;
        private readonly string? _toEvent;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandBinding{TViewModel, TParam, TResult}"/> class.
        /// </summary>
        /// <param name="viewModelBinding">Expression for the View Model binding.</param>
        /// <param name="toEvent">If specified, bind to the specific event instead of the default.</param>
        public CommandBinding(
            Expression<Func<TViewModel, ReactiveCommand<TParam, TResult>?>> viewModelBinding,
            string? toEvent = null)
        {
            _viewModelBinding = viewModelBinding;
            _toEvent = toEvent;
        }

        /// <inheritdoc/>
        public void ApplyBinding<TView>(
            Action<IDisposable> disposeAction,
            TView view,
            TViewModel viewModel,
            Expression<Func<TView, ICommand>> viewBinding)
            where TView : class, IViewFor<TViewModel>
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
        public void ApplyBinding<TView>(
            CompositeDisposable compositeDisposable,
            TView view,
            TViewModel viewModel,
            Expression<Func<TView, ICommand>> viewBinding)
            where TView : class, IViewFor<TViewModel>
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
