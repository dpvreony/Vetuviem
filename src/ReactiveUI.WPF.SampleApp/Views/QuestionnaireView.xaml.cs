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

        private void OnWhenActivated(Action<IDisposable> disposeWithAction)
        {
            new QuestionnaireViewBindingModels().ApplyBindings(
                disposeWithAction,
                this,
                this.ViewModel);
        }
    }
}
