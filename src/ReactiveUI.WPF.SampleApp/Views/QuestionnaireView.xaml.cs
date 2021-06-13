using System;
using ReactiveUI.WPF.SampleApp.ViewBindingModels;
using ReactiveUI.WPF.SampleApp.ViewModels;

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
            var bindings = vbm.GetBindings();
            foreach (var viewBindingModel in bindings)
            {
                viewBindingModel.ApplyBindings(
                    this,
                    this.ViewModel,
                    action);
            }
        }
    }
}
