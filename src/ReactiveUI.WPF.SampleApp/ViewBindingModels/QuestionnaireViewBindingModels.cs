using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Windows.Controls;
using ReactiveUI.WPF.SampleApp.ViewModels;
using ReactiveUI.WPF.SampleApp.Views;
using ReactiveUI.Wpf.ViewToViewModelBindings.System.Windows.Controls;
using Vetuviem.Core;

namespace ReactiveUI.WPF.SampleApp.ViewBindingModels
{
    public sealed class QuestionnaireViewBindingModels : IEnableViewToViewModelBindings<QuestionnaireView, QuestionnaireViewModel>
    {
        public TextBoxViewBindingModel<QuestionnaireView, QuestionnaireViewModel> ForenameTextBoxViewBindingModel
            => GetStandardTextBoxViewModel(
                vw => vw.Forename,
                vm => vm.Forename);

        public TextBoxViewBindingModel<QuestionnaireView, QuestionnaireViewModel> SurnameTextBoxViewBindingModel
            => GetStandardTextBoxViewModel(
                vw =>vw.Surname,
                vm => vm.Surname);

        public TextBoxViewBindingModel<QuestionnaireView, QuestionnaireViewModel> AnswerOneTextBoxViewBindingModel
            => GetStandardTextBoxViewModel(
                vw =>vw.AnswerOne,
                vm => vm.AnswerOne);

        public TextBoxViewBindingModel<QuestionnaireView, QuestionnaireViewModel> AnswerTwoTextBoxViewBindingModel
            => GetStandardTextBoxViewModel(
                vw =>vw.AnswerTwo,
                vm => vm.AnswerTwo);

        public TextBoxViewBindingModel<QuestionnaireView, QuestionnaireViewModel> AnswerThreeTextBoxViewBindingModel
            => GetStandardTextBoxViewModel(
                vw =>vw.AnswerThree,
                vm => vm.AnswerThree);

        public TextBoxViewBindingModel<QuestionnaireView, QuestionnaireViewModel> AnswerFourTextBoxViewBindingModel
            => GetStandardTextBoxViewModel(
                vw =>vw.AnswerFour,
                vm => vm.AnswerFour);

        public TextBoxViewBindingModel<QuestionnaireView, QuestionnaireViewModel> AnswerFiveTextBoxViewBindingModel
            => GetStandardTextBoxViewModel(
                vw =>vw.AnswerFive,
                vm => vm.AnswerFive);

        public IEnumerable<IViewBindingModel<QuestionnaireView, QuestionnaireViewModel>> GetBindings()
        {
            yield return (IViewBindingModel<QuestionnaireView, QuestionnaireViewModel>)ForenameTextBoxViewBindingModel;
            yield return (IViewBindingModel<QuestionnaireView, QuestionnaireViewModel>)SurnameTextBoxViewBindingModel;
            yield return (IViewBindingModel<QuestionnaireView, QuestionnaireViewModel>)AnswerOneTextBoxViewBindingModel;
            yield return (IViewBindingModel<QuestionnaireView, QuestionnaireViewModel>)AnswerTwoTextBoxViewBindingModel;
            yield return (IViewBindingModel<QuestionnaireView, QuestionnaireViewModel>)AnswerThreeTextBoxViewBindingModel;
            yield return (IViewBindingModel<QuestionnaireView, QuestionnaireViewModel>)AnswerFourTextBoxViewBindingModel;
            yield return (IViewBindingModel<QuestionnaireView, QuestionnaireViewModel>)AnswerFiveTextBoxViewBindingModel;
        }

        private static TextBoxViewBindingModel<QuestionnaireView, QuestionnaireViewModel> GetStandardTextBoxViewModel(
            Expression<Func<QuestionnaireView, TextBox>> viewExpression,
            Expression<Func<QuestionnaireViewModel, string>> viewModelTextExpression)
        {
            return new(viewExpression)
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
