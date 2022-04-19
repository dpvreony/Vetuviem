// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace Vetuviem.Core
{
    public abstract class AbstractControlBindingModel<TView, TViewModel, TControl>
        : IControlBindingModel<TView, TViewModel>
        where TView : class, ReactiveUI.IViewFor<TViewModel>
        where TViewModel : class, ReactiveUI.IReactiveObject
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vetuviemControlBindingExpression"></param>
        /// <exception cref="ArgumentNullException"></exception>
        protected AbstractControlBindingModel(System.Linq.Expressions.Expression<Func<TView, TControl>> vetuviemControlBindingExpression)
        {
            VetuviemControlBindingExpression = vetuviemControlBindingExpression ?? throw new ArgumentNullException(nameof(vetuviemControlBindingExpression));
        }

        public System.Linq.Expressions.Expression<Func<TView, TControl>> VetuviemControlBindingExpression { get; }

        public abstract void ApplyBindings(
            TView view, TViewModel viewModel,
            Action<IDisposable> disposeAction);
    }
}
