using System.Windows;
using ReactiveUI.WPF.SampleApp.ViewModels;
using ReactiveUI.WPF.SampleApp.Views;

namespace ReactiveUI.WPF.SampleApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
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
