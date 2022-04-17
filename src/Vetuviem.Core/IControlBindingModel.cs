using System;
using ReactiveUI;

namespace Vetuviem.Core
{
    /// <summary>
    /// Represents a control binding model.
    /// </summary>
    /// <typeparam name="TView">The type for the view.</typeparam>
    /// <typeparam name="TViewModel">The type for the viewmodel.</typeparam>
    public interface IControlBindingModel<in TView, in TViewModel>
        where TView : class, IViewFor<TViewModel>
        where TViewModel : class, IReactiveObject
    {
        void ApplyBindings(TView view, TViewModel viewModel, Action<IDisposable> disposeAction);
    }
}
