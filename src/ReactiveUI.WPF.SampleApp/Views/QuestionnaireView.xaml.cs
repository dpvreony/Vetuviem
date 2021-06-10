﻿using System;
using System.Linq.Expressions;
using System.Windows.Controls;
using ReactiveUI.WPF.SampleApp.ViewBindingModels;
using ReactiveUI.WPF.SampleApp.ViewModels;
using ReactiveUI.Wpf.ViewToViewModelBindingHelpers.System.Windows.Controls;
using ReactiveUI.Wpf.ViewToViewModelBindings.System.Windows.Controls;

namespace ReactiveUI.WPF.SampleApp.Views
{
    public class ReactiveQuestionnaireWindow : ReactiveWindow<QuestionnaireViewModel>
    {

    }

    /// <summary>
    /// Interaction logic for QuestionnaireView.xaml
    /// </summary>
    public partial class QuestionnaireView
    {
        public QuestionnaireView()
        {
            InitializeComponent();

            this.WhenActivated(action => this.OnWhenActivated(action));
        }

        private void OnWhenActivated(Action<IDisposable> action)
        {
            // this is still an investigation piece as the next bit to make more usable.
            var vbm = new QuestionnaireViewBindingModels();

            // we should have a get bindings method
            /*
            var bindingModels = vbm.GetBindingModels();
            foreach (var bindingModel in bindingModels)
            {
                bindingModel.ApplyBinding();
            }
            */

            // as we went to the trouble of producing a vbm, it should have an ApplyBinding() method on the IOneWay or IOneOrTwoWay interfaces
            vbm.ForenameTextBoxViewBindingModel.ApplyBinding(
                this,
                this.ViewModel,
                action,
                vw => vw.Forename);

            vbm.ForenameTextBoxViewBindingModel.ApplyBinding(
                this,
                this.ViewModel,
                action,
                vw => vw.Surname);
        }
    }

    public static class VbmExtensions
    {
        public static void ApplyBinding(
            this TextBoxViewBindingModel<QuestionnaireView, QuestionnaireViewModel> vbm,
            QuestionnaireView view,
            QuestionnaireViewModel viewModel,
            Action<IDisposable> action,
            Expression<Func<QuestionnaireView, TextBox>> control)
        {
            TextBoxViewBindingHelper.ApplyBinding(
                view,
                viewModel,
                vbm,
                action,
                control);

        }
    }
}