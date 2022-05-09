// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using ReactiveUI.WPF.SampleApp.ViewModels;
using ReactiveUI.WPF.SampleApp.Views;

namespace ReactiveUI.WPF.SampleApp
{
    /// <summary>
    /// Interaction logic for Application Entry Point.
    /// </summary>
    public partial class App : Application
    {
        /// <inheritdoc/>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var view = new QuestionnaireView
            {
                ViewModel = new QuestionnaireViewModel()
            };

            MainWindow = view;

            view.Show();
        }
    }
}
