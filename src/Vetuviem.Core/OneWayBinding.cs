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
    /// Represents a one way View and ViewModel binding.
    /// </summary>
    /// <typeparam name="TVetuviemTargetViewModel">The type for the target ViewModel that Vetuviem will bind to.</typeparam>
    /// <typeparam name="TViewProp">The type for the View.</typeparam>
    public class OneWayBinding<TVetuviemTargetViewModel, TViewProp> : IOneWayBind<TVetuviemTargetViewModel, TViewProp>
        where TVetuviemTargetViewModel : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OneWayBinding{TViewModel,TViewProp}"/> class.
        /// </summary>
        /// <param name="viewModelBinding">Expression for the View Model binding.</param>
        public OneWayBinding(Expression<Func<TVetuviemTargetViewModel, TViewProp?>> viewModelBinding)
        {
            ViewModelBinding = viewModelBinding ?? throw new ArgumentNullException(nameof(viewModelBinding));
        }

        /// <inheritdoc/>
        public Expression<Func<TVetuviemTargetViewModel, TViewProp?>> ViewModelBinding
        {
            get;
        }

        /// <inheritdoc/>
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
                viewBinding));
        }

        /// <inheritdoc/>
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
                viewBinding).DisposeWith(compositeDisposable);
        }
    }
}
