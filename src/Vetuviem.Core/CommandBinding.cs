using System;
using System.Linq.Expressions;
using System.Reactive;
using System.Windows.Input;
using ReactiveUI;

namespace Vetuviem.Core
{
    public sealed class CommandBinding<TViewModel> : ICommandBinding<TViewModel, ICommand>
        where TViewModel : class
    {
        private readonly Expression<Func<TViewModel, ReactiveCommand<Unit, Unit>>> _viewModelBinding;
        private readonly string _toEvent;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandBinding{TViewModel}"/> class.
        /// </summary>
        /// <param name="viewModelBinding">Expression for the View Model binding.</param>
        /// <param name="toEvent">If specified, bind to the specific event instead of the default.</param>
        public CommandBinding(
            Expression<Func<TViewModel, ReactiveCommand<Unit, Unit>>> viewModelBinding,
            string toEvent = null)
        {
            _viewModelBinding = viewModelBinding ?? throw new ArgumentNullException(nameof(viewModelBinding));
            _toEvent = toEvent;
        }

        /// <inheritdoc/>
        public void ApplyBinding<TView>(
            Action<IDisposable> disposeAction,
            TView view,
            TViewModel viewModel,
            Expression<Func<TView, ICommand>> viewBinding)
            where TView : class, IViewFor<TViewModel>
        {
            if (disposeAction == null)
            {
                throw new ArgumentNullException(nameof(disposeAction));
            }

            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            if (viewBinding == null)
            {
                throw new ArgumentNullException(nameof(viewBinding));
            }

            disposeAction(view.BindCommand(
                viewModel,
                _viewModelBinding,
                viewBinding,
                _toEvent));
        }
    }
}
