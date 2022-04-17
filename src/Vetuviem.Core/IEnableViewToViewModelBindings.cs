﻿using System;
using ReactiveUI;

namespace Vetuviem.Core
{
    /// <summary>
    /// Represents a View to View Model Binding.
    /// </summary>
    /// <typeparam name="TView"></typeparam>
    /// <typeparam name="TViewModel"></typeparam>
    public interface IEnableViewToViewModelBindings<in TView, in TViewModel>
        where TView : class, IViewFor<TViewModel>
        where TViewModel : class, IReactiveObject
    {
        /// <summary>
        /// Apply control bindings between a View and ViewModel
        /// </summary>
        /// <param name="disposeWithAction">The ReactiveUI Disposal Tracker.</param>
        /// <param name="view">Instance of the view.</param>
        /// <param name="viewModel">Instance of the viewmodel.</param>
        void ApplyBindings(
            Action<IDisposable> disposeWithAction,
            TView view,
            TViewModel viewModel);
    }
}
