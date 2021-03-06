// Copyright (c) 2020 DHGMS Solutions and Contributors. All rights reserved.
// DHGMS Solutions and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;
using ReactiveUI;

namespace Vetuviem.Core
{
    /// <summary>
    /// Represents a one way View and ViewModel binding.
    /// </summary>
    /// <typeparam name="TViewModel">The type for the ViewModel.</typeparam>
    /// <typeparam name="TViewProp">The type for the View.</typeparam>
    public class OneWayBinding<TViewModel, TViewProp> : IOneWayBind<TViewModel, TViewProp>
        where TViewModel : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OneWayBinding{TViewModel, TViewProp}"/> class.
        /// </summary>
        /// <param name="viewModelBinding">Expression for the View Model binding.</param>
        public OneWayBinding(Expression<Func<TViewModel, TViewProp?>> viewModelBinding)
        {
            ViewModelBinding = viewModelBinding ?? throw new ArgumentNullException(nameof(viewModelBinding));
        }

        /// <inheritdoc/>
        public Expression<Func<TViewModel, TViewProp?>> ViewModelBinding
        {
            get;
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

            d(view.OneWayBind(
                viewModel,
                ViewModelBinding,
                viewBinding));
        }
    }

    /// <summary>
    /// Represents a one way View and ViewModel binding.
    /// </summary>
    /// <typeparam name="TViewModel">The type for the ViewModel.</typeparam>
    /// <typeparam name="TViewProp">The type for the View.</typeparam>
    /// <typeparam name="TOut">The type for the control binding.</typeparam>
    public class OneWayBindingWithConversion<TViewModel, TViewProp, TOut> : IOneWayBind<TViewModel, TViewProp>
        where TViewModel : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OneWayBinding{TViewModel, TViewProp}"/> class.
        /// </summary>
        /// <param name="viewModelBinding">Expression for the View Model binding.</param>
        /// <param name="selector">Conversion selector function.</param>
        public OneWayBindingWithConversion(
            Expression<Func<TViewModel, TViewProp?>> viewModelBinding,
            Func<TViewProp, TOut> selector)
        {
            ViewModelBinding = viewModelBinding ?? throw new ArgumentNullException(nameof(viewModelBinding));
            Selector = selector ?? throw new ArgumentNullException(nameof(selector));
        }

        /// <summary>
        /// Gets the conversion selector function.
        /// </summary>
        public Func<TViewProp, TOut> Selector { get; }

        /// <inheritdoc/>
        public Expression<Func<TViewModel, TViewProp?>> ViewModelBinding
        {
            get;
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

            d(view.OneWayBind(
                viewModel,
                ViewModelBinding,
                viewBinding,
                Selector));
        }
    }

    /// <summary>
    /// Represents a one way View and ViewModel binding.
    /// </summary>
    /// <typeparam name="TViewModel">The type for the ViewModel.</typeparam>
    /// <typeparam name="TViewProp">The type for the View.</typeparam>
    /// <typeparam name="TOut">The type for the control binding.</typeparam>
    public class OneWayBindingWithConversionOnOneOrTwoWay<TViewModel, TViewProp, TVMProp> : IOneOrTwoWayBind<TViewModel, TViewProp>
        where TViewModel : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OneWayBinding{TViewModel, TViewProp}"/> class.
        /// </summary>
        /// <param name="viewModelBinding">Expression for the View Model binding.</param>
        /// <param name="selector">Conversion selector function.</param>
        public OneWayBindingWithConversionOnOneOrTwoWay(
            Expression<Func<TViewModel, TVMProp?>> viewModelBinding,
            Func<TVMProp?, TViewProp> selector)
        {
            ViewModelBinding = viewModelBinding ?? throw new ArgumentNullException(nameof(viewModelBinding));
            Selector = selector ?? throw new ArgumentNullException(nameof(selector));
        }

        /// <summary>
        /// Gets the conversion selector function.
        /// </summary>
        public Func<TVMProp?, TViewProp> Selector { get; }

        /// <inheritdoc/>
        public Expression<Func<TViewModel, TVMProp?>> ViewModelBinding
        {
            get;
        }

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

            d(view.OneWayBind(
                viewModel,
                ViewModelBinding,
                viewBinding,
                Selector));
        }

    }
}
