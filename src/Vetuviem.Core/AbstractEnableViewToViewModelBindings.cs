// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Windows.Input;
using ReactiveUI;

namespace Vetuviem.Core
{
    /// <summary>
    /// Abstraction for View to View model binding. This stores a collection of control model bindings.
    /// It's intended as a single point for invoking the binding to be applied on a view.
    /// </summary>
    /// <typeparam name="TView">The type for the view.</typeparam>
    /// <typeparam name="TVetuviemTargetViewModel">The type for the target viewmodel that Vetuviem will bind to.</typeparam>
    public abstract class AbstractEnableViewToViewModelBindings<TView, TVetuviemTargetViewModel> : IEnableViewToViewModelBindings<TView, TVetuviemTargetViewModel>
        where TView : class, IViewFor<TVetuviemTargetViewModel>
        where TVetuviemTargetViewModel : class, IReactiveObject
    {
        /// <inheritdoc />
        public void ApplyBindings(
            Action<IDisposable> disposeWithAction,
            TView view,
            TVetuviemTargetViewModel viewModel,
            IScheduler? scheduler = null)
        {
            if (disposeWithAction == null)
            {
                throw new ArgumentNullException(nameof(disposeWithAction));
            }

            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            var bindings = GetBindings();
            foreach (var viewBindingModel in bindings)
            {
                viewBindingModel.ApplyBindings(
                    view,
                    viewModel,
                    disposeWithAction);
            }

            var subscriptions = GetSubscriptions(
                view,
                viewModel,
                scheduler);

            foreach (var subscription in subscriptions)
            {
                disposeWithAction(subscription);
            }
        }

        /// <inheritdoc />
        public void ApplyBindings(
            CompositeDisposable compositeDisposable,
            TView view,
            TVetuviemTargetViewModel viewModel,
            IScheduler? scheduler = null)
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

            var bindings = GetBindings();
            foreach (var viewBindingModel in bindings)
            {
                viewBindingModel.ApplyBindings(
                    view,
                    viewModel,
                    compositeDisposable);
            }

            var subscriptions = GetSubscriptions(
                view,
                viewModel,
                scheduler);

            foreach (var subscription in subscriptions)
            {
                compositeDisposable.Add(subscription);
            }
        }

        /// <summary>
        /// Gets the controls to be bound on the view.
        /// </summary>
        /// <returns>Collection of control to viewmodel bindings.</returns>
        protected abstract IEnumerable<IControlBindingModel<TView, TVetuviemTargetViewModel>> GetBindings();

        /// <summary>
        /// Gets the subscriptions to be bound between the View and the ViewModel. Can be used to subscribe the View to ViewModel Commands or Interactions. This is separate from the control bindings as it allows for more complex bindings that may not be easily represented by a control binding model. For example Command subscriptions can be used to execute methods in the View.
        /// </summary>
        /// <remarks>You do not need to wire up disposal logic, this is handled internally.</remarks>
        /// <returns>Collection of subscriptions.</returns>
        protected abstract IEnumerable<IDisposable> GetSubscriptions(TView view, TVetuviemTargetViewModel viewModel, IScheduler? scheduler);

        /// <summary>
        /// Gets an expression for a view property. This is intended to be used in the implementation of <see cref="GetBindings"/> to provide a strongly typed shorthand way of specifying the view properties to bind to.
        /// </summary>
        /// <typeparam name="TViewProp">The type for the property on the view.</typeparam>
        /// <param name="viewProperty">The expression representing the view property.</param>
        /// <returns>The expression for the view property.</returns>
        protected static Expression<Func<TView, TViewProp>> ForViewProperty<TViewProp>(Expression<Func<TView, TViewProp>> viewProperty) => viewProperty;

        /// <summary>
        /// Gets an expression for a view model command. This is intended to be used in the implementation of <see cref="GetBindings"/> to provide a strongly typed shorthand way of specifying the view model commands to bind to.
        /// </summary>
        /// <param name="viewModelCommand">The expression representing the view model property.</param>
        /// <returns>The expression for the view model command.</returns>
        protected static Expression<Func<TVetuviemTargetViewModel, ICommand?>> ForViewModelCommand(Expression<Func<TVetuviemTargetViewModel, ICommand?>> viewModelCommand) => viewModelCommand;

        /// <summary>
        /// Gets an expression for a view model property. This is intended to be used in the implementation of <see cref="GetBindings"/> to provide a strongly typed shorthand way of specifying the view model properties to bind to.
        /// </summary>
        /// <typeparam name="TViewModelProp">The type for the property on the view model.</typeparam>
        /// <param name="viewModelProperty">The expression representing the view model property.</param>
        /// <returns>The expression for the view model property.</returns>
        protected static Expression<Func<TVetuviemTargetViewModel, TViewModelProp>> ForViewModelProperty<TViewModelProp>(Expression<Func<TVetuviemTargetViewModel, TViewModelProp>> viewModelProperty) => viewModelProperty;

        /// <summary>
        /// Gets a command binding for a view model command. This is intended to be used in the implementation of <see cref="GetBindings"/> to provide a strongly typed shorthand way of specifying the view model commands to bind to.
        /// </summary>
        /// <param name="viewModelBinding">The expression representing the view model property.</param>
        /// <returns>The command binding for the view model command.</returns>
        protected static ICommandBinding<TVetuviemTargetViewModel> GetCommandBinding(Expression<Func<TVetuviemTargetViewModel, ICommand?>> viewModelBinding)
        {
            return new CommandBinding<TVetuviemTargetViewModel>(viewModelBinding);
        }
    }
}
