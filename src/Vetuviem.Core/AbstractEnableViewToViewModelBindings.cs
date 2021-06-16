using System;
using System.Collections.Generic;
using ReactiveUI;

namespace Vetuviem.Core
{
    public abstract class AbstractEnableViewToViewModelBindings<TView, TViewModel> : IEnableViewToViewModelBindings<TView, TViewModel>
        where TView : class, IViewFor<TViewModel>
        where TViewModel : class, IReactiveObject
    {
        public void ApplyBindings(
            Action<IDisposable> disposeWithAction,
            TView view,
            TViewModel viewModel)
        {
            var bindings = this.GetBindings();
            foreach (var viewBindingModel in bindings)
            {
                viewBindingModel.ApplyBindings(
                    view,
                    viewModel,
                    disposeWithAction);
            }
        }

        protected abstract IEnumerable<IViewBindingModel<TView, TViewModel>> GetBindings();
    }
}
