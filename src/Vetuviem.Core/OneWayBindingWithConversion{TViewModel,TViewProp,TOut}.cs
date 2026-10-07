// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.


using System;
using System.Linq.Expressions;
using ReactiveUI.Primitives;
using ReactiveUI.Primitives.Disposables;

namespace Vetuviem.Core
{
    /// <summary>
    /// Represents a one way View and ViewModel binding.
    /// </summary>
    /// <typeparam name="TVetuviemTargetViewModel">The type for the target ViewModel that Vetuviem will bind to.</typeparam>
    /// <typeparam name="TViewProp">The type for the View.</typeparam>
    /// <typeparam name="TOut">The type for the control binding.</typeparam>
    public class OneWayBindingWithConversion<TVetuviemTargetViewModel, TViewProp, TOut> : IOneWayBind<TVetuviemTargetViewModel, TOut>
        where TVetuviemTargetViewModel : class
    {
        private readonly Expression<Func<TVetuviemTargetViewModel, TViewProp>> _viewModelBinding;

        /// <summary>
        /// Initializes a new instance of the <see cref="OneWayBindingWithConversion{TViewModel,TViewProp, TOut}"/> class.
        /// </summary>
        /// <param name="viewModelBinding">Expression for the View Model binding.</param>
        /// <param name="selector">Conversion selector function.</param>
        public OneWayBindingWithConversion(
            Expression<Func<TVetuviemTargetViewModel, TViewProp>> viewModelBinding,
            Func<TViewProp, TOut> selector)
        {
            _viewModelBinding = viewModelBinding ?? throw new ArgumentNullException(nameof(viewModelBinding));
            Selector = selector ?? throw new ArgumentNullException(nameof(selector));
        }

        /// <summary>
        /// Gets the conversion selector function.
        /// </summary>
        public Func<TViewProp, TOut> Selector { get; }

        /// <inheritdoc/>
        public void ApplyBinding<TView>(
            Action<IDisposable> d,
            TView view,
            TVetuviemTargetViewModel viewModel,
            Expression<Func<TView, TOut>> viewBinding)
            where TView : class, IViewFor<TVetuviemTargetViewModel>
        {
            if (d == null)
            {
                throw new ArgumentNullException(nameof(d));
            }

            d(view.OneWayBind(
                viewModel: viewModel,
                viewModelProperty: _viewModelBinding,
                viewProperty: viewBinding,
                selector: Selector));
        }

        /// <inheritdoc/>
        public void ApplyBinding<TView>(
            MultipleDisposable multipleDisposable,
            TView view,
            TVetuviemTargetViewModel viewModel,
            Expression<Func<TView, TOut>> viewBinding)
            where TView : class, IViewFor<TVetuviemTargetViewModel>
        {
            if (multipleDisposable == null)
            {
                throw new ArgumentNullException(nameof(multipleDisposable));
            }

            view.OneWayBind(
                viewModel: viewModel,
                viewModelProperty: _viewModelBinding,
                viewProperty: viewBinding,
                selector: Selector).DisposeWith(multipleDisposable);
        }
    }
}
