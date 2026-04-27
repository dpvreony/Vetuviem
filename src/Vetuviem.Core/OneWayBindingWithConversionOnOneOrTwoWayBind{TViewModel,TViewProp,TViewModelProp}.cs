// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using ReactiveUI;

namespace Vetuviem.Core
{
    /// <summary>
    /// Represents a one way View and ViewModel binding that applies a selection.
    /// </summary>
    /// <typeparam name="TVetuviemTargetViewModel">The type for the target ViewModel that Vetuviem will bind to.</typeparam>
    /// <typeparam name="TViewProp">The type for the View.</typeparam>
    /// <typeparam name="TViewModelProp">The type for the View Model Property.</typeparam>
    public class OneWayBindingWithConversionOnOneOrTwoWayBind<TVetuviemTargetViewModel, TViewProp, TViewModelProp> : IOneOrTwoWayBind<TVetuviemTargetViewModel, TViewProp>
        where TVetuviemTargetViewModel : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OneWayBindingWithConversionOnOneOrTwoWayBind{TViewModel,TViewProp,TOut}"/> class.
        /// </summary>
        /// <param name="viewModelBinding">Expression for the View Model binding.</param>
        /// <param name="selector">Conversion selector function.</param>
        public OneWayBindingWithConversionOnOneOrTwoWayBind(
            Expression<Func<TVetuviemTargetViewModel, TViewModelProp?>> viewModelBinding,
            Func<TViewModelProp?, TViewProp> selector)
        {
            ViewModelBinding = viewModelBinding ?? throw new ArgumentNullException(nameof(viewModelBinding));
            Selector = selector ?? throw new ArgumentNullException(nameof(selector));
        }

        /// <summary>
        /// Gets the conversion selector function.
        /// </summary>
        public Func<TViewModelProp?, TViewProp> Selector { get; }

        /// <inheritdoc/>
        public Expression<Func<TVetuviemTargetViewModel, TViewModelProp?>> ViewModelBinding
        {
            get;
        }

        /// <inheritdoc />
        public void ApplyBinding<TView>(
            Action<IDisposable> d,
            TView view,
            TVetuviemTargetViewModel viewModel,
            Expression<Func<TView, TViewProp>> viewBinding)
            where TView : class, IViewFor<TVetuviemTargetViewModel>
        {
            if (d == null)
            {
                throw new ArgumentNullException(nameof(d));
            }

            d(view.OneWayBind(
                viewModel,
                ViewModelBinding,
                viewBinding,
                Selector));
        }

        /// <inheritdoc />
        public void ApplyBinding<TView>(
            CompositeDisposable compositeDisposable,
            TView view,
            TVetuviemTargetViewModel viewModel,
            Expression<Func<TView, TViewProp>> viewBinding)
            where TView : class, IViewFor<TVetuviemTargetViewModel>
        {
            if (compositeDisposable == null)
            {
                throw new ArgumentNullException(nameof(compositeDisposable));
            }

            view.OneWayBind(
                viewModel,
                ViewModelBinding,
                viewBinding,
                Selector).DisposeWith(compositeDisposable);
        }
    }
}
