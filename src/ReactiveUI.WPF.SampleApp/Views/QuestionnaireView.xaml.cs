// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.SimpleChildWindow;
using ReactiveUI.WPF.SampleApp.ViewBindingModels;

namespace ReactiveUI.WPF.SampleApp.Views
{
    /// <summary>
    /// Interaction logic for Questionnaire View.
    /// </summary>
    public partial class QuestionnaireView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionnaireView"/> class.
        /// </summary>
        public QuestionnaireView()
        {
            InitializeComponent();

            // ReSharper disable once ConvertClosureToMethodGroup
            this.WhenActivated(action => OnWhenActivated(action));
        }

        private void OnWhenActivated(Action<IDisposable> disposeWithAction)
        {
            new QuestionnaireViewBindingModels().ApplyBindings(
                disposeWithAction,
                this,
                ViewModel!);
        }

        internal async Task ShowChildWindowInteractionAsync()
        {
            var childWindowView = new ChildWindow();
            await this.ShowChildWindowAsync(childWindowView);
        }
    }
}
