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
    }
}
