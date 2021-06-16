using System;
using System.Collections.Generic;
using ReactiveUI;

namespace Vetuviem.Core
{
    public interface IEnableViewToViewModelBindings<in TView, in TViewModel>
        where TView : class, IViewFor<TViewModel>
        where TViewModel : class, IReactiveObject
    {
        void ApplyBindings(
            Action<IDisposable> disposeWithAction,
            TView view,
            TViewModel viewModel);
    }
}
