using System;
using System.Linq.Expressions;
using ReactiveUI;

namespace Vetuviem.Core
{
    /// <summary>
    /// Represents a View to View Model Binding for Commands.
    /// </summary>
    /// <typeparam name="TViewModel">The type for the ViewModel.</typeparam>
    /// <typeparam name="TViewProp">The type for the View.</typeparam>
    public interface ICommandBinding<TViewModel, TViewProp>
        where TViewModel : class
    {
        /// <summary>
        /// Applies a View to View Model Binding.
        /// </summary>
        /// <typeparam name="TView">The type for the view.</typeparam>
        /// <param name="d">The disposable action registration. Used to clean up when bindings fall out of scope.</param>
        /// <param name="view">The instance of the View to bind.</param>
        /// <param name="viewModel">The instance of the ViewModel to Bind.</param>
        /// <param name="viewBinding">Expression of the View Property to Bind to.</param>
        void ApplyBinding<TView>(
            Action<IDisposable> d,
            TView view,
            TViewModel viewModel,
            Expression<Func<TView, TViewProp>> viewBinding)
            where TView : class, IViewFor<TViewModel>;
    }
}
