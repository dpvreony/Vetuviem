using System;

namespace Vetuviem.Core
{
    public abstract class AbstractViewBindingModel<TView, TViewModel, TControl>
        : IViewBindingModel<TView, TViewModel>
        where TView : class, ReactiveUI.IViewFor<TViewModel>
        where TViewModel : class, ReactiveUI.IReactiveObject
    {
        protected AbstractViewBindingModel(global::System.Linq.Expressions.Expression<Func<TView, TControl>> vetuviemControlBindingExpression)
        {
            VetuviemControlBindingExpression = vetuviemControlBindingExpression ?? throw new ArgumentNullException(nameof(vetuviemControlBindingExpression));
        }

        public System.Linq.Expressions.Expression<Func<TView, TControl>> VetuviemControlBindingExpression { get; }

        public abstract void ApplyBindings(
            TView view, TViewModel viewModel,
            Action<IDisposable> disposeAction);
    }
}
