// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace Vetuviem.Core
{
    /// <summary>
    /// Abstraction of a Control Binding Model.
    /// </summary>
    /// <typeparam name="TView">The type for the view.</typeparam>
    /// <typeparam name="TViewModel">The type for the viewmodel.</typeparam>
    /// <typeparam name="TControl">The type for the control.</typeparam>
    public abstract class AbstractControlBindingModel<TView, TViewModel, TControl>
        : IControlBindingModel<TView, TViewModel>
        where TView : class, ReactiveUI.IViewFor<TViewModel>
        where TViewModel : class, ReactiveUI.IReactiveObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractControlBindingModel{TView, TViewModel, TControl}"/> class.
        /// </summary>
        /// <param name="vetuviemControlBindingExpression">An expression representing the control on the view which will have the binding applied to it.</param>
        protected AbstractControlBindingModel(System.Linq.Expressions.Expression<Func<TView, TControl>> vetuviemControlBindingExpression)
        {
            VetuviemControlBindingExpression = vetuviemControlBindingExpression ?? throw new ArgumentNullException(nameof(vetuviemControlBindingExpression));
        }

        /// <summary>
        /// Gets an expression representing the control on the view which will have the binding applied to it.
        /// </summary>
        public System.Linq.Expressions.Expression<Func<TView, TControl>> VetuviemControlBindingExpression { get; }

        /// <inheritdoc/>
        public abstract void ApplyBindings(
            TView view,
            TViewModel viewModel,
            Action<IDisposable> disposeAction);
    }
}
