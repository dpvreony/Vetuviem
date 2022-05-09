// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;
using ReactiveUI;

namespace Vetuviem.Core
{
    /// <summary>
    /// Represents a Two way binding on a View Property that supports One or Two way binding.
    /// </summary>
    /// <typeparam name="TViewModel">The type for the ViewModel.</typeparam>
    /// <typeparam name="TViewProp">The type for the View Property.</typeparam>
    /// <typeparam name="TViewModelProp">The type for the View Model Property.</typeparam>
    public class TwoWayBindingWithConvertors<TViewModel, TViewProp, TViewModelProp> : IOneOrTwoWayBind<TViewModel, TViewProp>
        where TViewModel : class
    {
        private readonly Expression<Func<TViewModel, TViewModelProp?>> _viewModelBinding;
        private readonly Func<TViewModelProp?, TViewProp> _vmToViewConverter;
        private readonly Func<TViewProp, TViewModelProp> _viewToVmConverter;

        /// <summary>
        /// Initializes a new instance of the <see cref="TwoWayBindingWithConvertors{TViewModel, TViewProp, TViewModelProp}"/> class.
        /// </summary>
        /// <param name="viewModelBinding">Expression for the View Model binding.</param>
        /// <param name="vmToViewConverter">Function for converting the ViewModel property to the type of the View Property.</param>
        /// <param name="viewToVmConverter">Function for converting the View property to the type of the ViewModel Property.</param>
        public TwoWayBindingWithConvertors(
            Expression<Func<TViewModel, TViewModelProp?>> viewModelBinding,
            Func<TViewModelProp?, TViewProp> vmToViewConverter,
            Func<TViewProp, TViewModelProp> viewToVmConverter)
        {
            _viewModelBinding = viewModelBinding ?? throw new ArgumentNullException(nameof(viewModelBinding));
            _vmToViewConverter = vmToViewConverter ?? throw new ArgumentNullException(nameof(vmToViewConverter));
            _viewToVmConverter = viewToVmConverter ?? throw new ArgumentNullException(nameof(viewToVmConverter));
        }

        /// <inheritdoc/>
        public void ApplyBinding<TView>(
            Action<IDisposable> d,
            TView view,
            TViewModel viewModel,
            Expression<Func<TView, TViewProp>> viewBinding)
            where TView : class, IViewFor<TViewModel>
        {
            if (d == null)
            {
                throw new ArgumentNullException(nameof(d));
            }

            d(view.Bind(
                viewModel,
                _viewModelBinding,
                viewBinding,
                _vmToViewConverter,
                _viewToVmConverter));
        }
    }
}
