using System;
using System.Linq.Expressions;
using ReactiveUI;

namespace Vetuviem.Core
{
    public class OneWayBindingOnOneOrTwoWayBind<TViewModel, TViewProp> : IOneOrTwoWayBind<TViewModel, TViewProp>
        where TViewModel : class
    {
        private readonly Expression<Func<TViewModel, TViewProp?>> _viewModelBinding;
        private readonly Func<TViewProp?, TViewProp> _vmToViewConverter;

        /// <summary>
        /// Initializes a new instance of the <see cref="OneWayBindingOnOneOrTwoWayBind{TViewModel,TViewProp}"/> class.
        /// </summary>
        /// <param name="viewModelBinding">Expression for the View Model binding.</param>
        public OneWayBindingOnOneOrTwoWayBind(
            Expression<Func<TViewModel, TViewProp?>> viewModelBinding,
            Func<TViewProp?, TViewProp> vmToViewConverter)
        {
            _viewModelBinding = viewModelBinding ?? throw new ArgumentNullException(nameof(viewModelBinding));
            _vmToViewConverter = vmToViewConverter;
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
                _viewModelBinding,
                viewBinding,
                _vmToViewConverter));
        }
    }

    /// <summary>
    /// Represents a one way View and ViewModel binding that applies a selection.
    /// </summary>
    /// <typeparam name="TViewModel">The type for the ViewModel.</typeparam>
    /// <typeparam name="TViewProp">The type for the View.</typeparam>
    /// <typeparam name="TViewModelProp">The type for the View Model Property.</typeparam>
    public class OneWayBindingWithConversionOnOneOrTwoWayBind<TViewModel, TViewProp, TViewModelProp> : IOneOrTwoWayBind<TViewModel, TViewProp>
        where TViewModel : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OneWayBindingWithConversionOnOneOrTwoWayBind{TViewModel,TViewProp,TOut}"/> class.
        /// </summary>
        /// <param name="viewModelBinding">Expression for the View Model binding.</param>
        /// <param name="selector">Conversion selector function.</param>
        public OneWayBindingWithConversionOnOneOrTwoWayBind(
            Expression<Func<TViewModel, TViewModelProp?>> viewModelBinding,
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
        public Expression<Func<TViewModel, TViewModelProp?>> ViewModelBinding
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
