using System;

namespace Vetuviem.Core
{
    public abstract class AbstractViewBindingModel<TView, TViewModel, TControl>
        : global::Vetuviem.Core.IViewBindingModel<TView, TViewModel>
        where TView : class, global::ReactiveUI.IViewFor<TViewModel>
        where TViewModel : class, global::ReactiveUI.IReactiveObject
    {
        protected AbstractViewBindingModel(global::System.Linq.Expressions.Expression<global::System.Func<TView, TControl>> vetuviemControlBindingExpression)
        {
            VetuviemControlBindingExpression = vetuviemControlBindingExpression ?? throw new ArgumentNullException(nameof(vetuviemControlBindingExpression));
        }

        public global::System.Linq.Expressions.Expression<global::System.Func<TView, TControl>> VetuviemControlBindingExpression { get; }

        public abstract void ApplyBindings(TView view, TViewModel viewModel, Action<IDisposable> disposeAction);
    }
}
