using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using ReactiveUI;

namespace Vetuviem.Core
{
    public class OneWayBind<TViewModel, TViewProp> : IOneOrTwoWayBind<TViewModel, TViewProp>
        where TViewModel : class
    {
        private readonly Expression<Func<TViewModel, TViewProp>> _viewModelBinding;
        private readonly Func<TViewProp, TViewProp> _vmToViewConverter;

        /// <summary>
        /// Initializes a new instance of the <see cref="OneWayBind{TViewModel, TViewProp}"/> class.
        /// </summary>
        /// <param name="viewModelBinding">Expression for the View Model binding.</param>
        public OneWayBind(
            Expression<Func<TViewModel, TViewProp>> viewModelBinding,
            Func<TViewProp, TViewProp> vmToViewConverter)
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
}
