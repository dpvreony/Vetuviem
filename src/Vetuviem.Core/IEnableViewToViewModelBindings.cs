using System.Collections.Generic;
using ReactiveUI;

namespace Vetuviem.Core
{
    public interface IEnableViewToViewModelBindings<TView, TViewModel>
        where TView : class, IViewFor<TViewModel>
        where TViewModel : class, IReactiveObject
    {
        IEnumerable<IViewBindingModel<TView, TViewModel>> GetBindings();
    }
}
