using System;
using System.Linq.Expressions;
using ReactiveUI.WPF.SampleApp.ViewModels;
using ReactiveUI.WPF.SampleApp.Views;
using ReactiveUI.Wpf.ViewToViewModelBindings.System.Windows.Controls;
using Vetuviem.Core;

namespace ReactiveUI.WPF.SampleApp.ViewBindingModels
{
    public sealed class QuestionnaireViewBindingModels
    {
        public TextBoxViewBindingModel<QuestionnaireView, QuestionnaireViewModel> ForenameTextBoxViewBindingModel
            => GetStandardTextBoxViewModel(vm => vm.Forename);

        public TextBoxViewBindingModel<QuestionnaireView, QuestionnaireViewModel> SurnameTextBoxViewBindingModel
            => GetStandardTextBoxViewModel(vm => vm.Surname);

        public TextBoxViewBindingModel<QuestionnaireView, QuestionnaireViewModel> AnswerOneTextBoxViewBindingModel
            => GetStandardTextBoxViewModel(vm => vm.AnswerOne);

        public TextBoxViewBindingModel<QuestionnaireView, QuestionnaireViewModel> AnswerTwoTextBoxViewBindingModel
            => GetStandardTextBoxViewModel(vm => vm.AnswerTwo);

        public TextBoxViewBindingModel<QuestionnaireView, QuestionnaireViewModel> AnswerThreeTextBoxViewBindingModel
            => GetStandardTextBoxViewModel(vm => vm.AnswerThree);

        public TextBoxViewBindingModel<QuestionnaireView, QuestionnaireViewModel> AnswerFourTextBoxViewBindingModel
            => GetStandardTextBoxViewModel(vm => vm.AnswerFour);

        public TextBoxViewBindingModel<QuestionnaireView, QuestionnaireViewModel> AnswerFiveTextBoxViewBindingModel
            => GetStandardTextBoxViewModel(vm => vm.AnswerFive);

        private static TextBoxViewBindingModel<QuestionnaireView, QuestionnaireViewModel> GetStandardTextBoxViewModel(Expression<Func<QuestionnaireViewModel, string>> viewModelTextExpression)
        {
            return new()
            {
                // max length is used in this scenario because you may have a flexible data capture form
                // in this sample it's fixed, but also used for calculating the limit on the label
                // we actually set the control max length slightly longer than the desired max length
                // this is to do with capturing a bad paste from the user. where it would cap at the max length of the
                // control, and they wouldn't be aware they passed the limit and lost data.
                MaxLength = new OneWayBind<QuestionnaireViewModel, int>(vm => vm.MaxLength, x => x + 10),
                Text = new TwoWayBinding<QuestionnaireViewModel, string>(viewModelTextExpression),
            };
        }
    }
}
