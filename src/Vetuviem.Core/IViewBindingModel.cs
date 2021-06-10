using ReactiveUI;

namespace Vetuviem.Core
{
    public interface IViewBindingModel<TView, TViewModel>
        where TView : class, IViewFor<TViewModel>
        where TViewModel : class, IReactiveObject
    {
    }
}
