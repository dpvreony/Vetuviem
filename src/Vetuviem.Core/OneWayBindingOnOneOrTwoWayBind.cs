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
    /// Represents a One Way Bind to a property that takes One or Two way binding.
    /// </summary>
    /// <typeparam name="TVetuviemTargetViewModel">The type for the target ViewModel that Vetuviem will bind to.</typeparam>
    /// <typeparam name="TViewProp">The type for the View Property.</typeparam>
    public class OneWayBindingOnOneOrTwoWayBind<TVetuviemTargetViewModel, TViewProp> : IOneOrTwoWayBind<TVetuviemTargetViewModel, TViewProp>
        where TVetuviemTargetViewModel : class
    {
        private readonly Expression<Func<TVetuviemTargetViewModel, TViewProp?>> _viewModelBinding;
        private readonly Func<TViewProp?, TViewProp> _vmToViewConverter;

        /// <summary>
        /// Initializes a new instance of the <see cref="OneWayBindingOnOneOrTwoWayBind{TViewModel,TViewProp}"/> class.
        /// </summary>
        /// <param name="viewModelBinding">Expression for the View Model binding.</param>
        /// <param name="vmToViewConverter">Function for converting the ViewModel property to the type of the View Property.</param>
        public OneWayBindingOnOneOrTwoWayBind(
            Expression<Func<TVetuviemTargetViewModel, TViewProp?>> viewModelBinding,
            Func<TViewProp?, TViewProp> vmToViewConverter)
        {
            _viewModelBinding = viewModelBinding ?? throw new ArgumentNullException(nameof(viewModelBinding));
            _vmToViewConverter = vmToViewConverter;
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
                _viewModelBinding,
                viewBinding,
                _vmToViewConverter));
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
                _viewModelBinding,
                viewBinding,
                _vmToViewConverter).DisposeWith(compositeDisposable);
        }
    }
}
