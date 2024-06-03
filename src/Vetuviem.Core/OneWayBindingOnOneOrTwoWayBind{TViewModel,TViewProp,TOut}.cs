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
    /// Represents a one way View and ViewModel binding on a View Property that supports one or two way binding.
    /// </summary>
    /// <typeparam name="TViewModel">The type for the ViewModel.</typeparam>
    /// <typeparam name="TViewProp">The type for the View.</typeparam>
    /// <typeparam name="TOut">The type for the conversion.</typeparam>
    public class OneWayBindingOnOneOrTwoWayBind<TViewModel, TViewProp, TOut> : IOneOrTwoWayBind<TViewModel, TOut>
        where TViewModel : class
    {
        private readonly Expression<Func<TViewModel, TViewProp?>> _viewModelBinding;

        private readonly Expression<Func<TViewProp, TOut>> _converterBinding;

        /// <summary>
        /// Initializes a new instance of the <see cref="OneWayBindingOnOneOrTwoWayBind{TViewModel, TViewProp, TOut}"/> class.
        /// </summary>
        /// <param name="viewModelBinding">View to ViewModel binding expression.</param>
        /// <param name="converterBinding">View Property Conversion binding expression.</param>
        public OneWayBindingOnOneOrTwoWayBind(
            Expression<Func<TViewModel, TViewProp?>> viewModelBinding,
            Expression<Func<TViewProp, TOut>> converterBinding)
        {
            _viewModelBinding = viewModelBinding ?? throw new ArgumentNullException(nameof(viewModelBinding));
            _converterBinding = converterBinding ?? throw new ArgumentNullException(nameof(converterBinding));
        }

        /// <inheritdoc/>
        public void ApplyBinding<TView>(
            Action<IDisposable> d,
            TView view,
            TViewModel viewModel,
            Expression<Func<TView, TOut>> viewBinding)
            where TView : class, IViewFor<TViewModel>
        {
            if (d == null)
            {
                throw new ArgumentNullException(nameof(d));
            }

            d(view.OneWayBind(
                viewModel,
                _viewModelBinding,
                viewBinding,
                _converterBinding));
        }

        /// <inheritdoc/>
        public void ApplyBinding<TView>(
            CompositeDisposable compositeDisposable,
            TView view,
            TViewModel viewModel,
            Expression<Func<TView, TOut>> viewBinding)
            where TView : class, IViewFor<TViewModel>
        {
            if (compositeDisposable == null)
            {
                throw new ArgumentNullException(nameof(compositeDisposable));
            }

            view.OneWayBind(
                viewModel,
                _viewModelBinding,
                viewBinding,
                _converterBinding).DisposeWith(compositeDisposable);
        }
    }
}
