// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using ReactiveUI;

namespace Vetuviem.Core
{
    public abstract class AbstractEnableViewToViewModelBindings<TView, TViewModel> : IEnableViewToViewModelBindings<TView, TViewModel>
        where TView : class, IViewFor<TViewModel>
        where TViewModel : class, IReactiveObject
    {
        /// <inheritdoc />
        public void ApplyBindings(
            Action<IDisposable> disposeWithAction,
            TView view,
            TViewModel viewModel)
        {
            var bindings = GetBindings();
            foreach (var viewBindingModel in bindings)
            {
                viewBindingModel.ApplyBindings(
                    view,
                    viewModel,
                    disposeWithAction);
            }
        }

        protected abstract IEnumerable<IControlBindingModel<TView, TViewModel>> GetBindings();
    }
}
