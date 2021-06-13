using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Windows.Controls;
using System.Windows.Media;
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

        public LabelViewBindingModel<QuestionnaireView, QuestionnaireViewModel> ForenameLengthRemainingLabelViewBindingModel
            => GetStandardLengthRemainingLabelViewBindingModel(
                vw => vw.ForenameLengthRemaining,
                vm => vm.ForenameLengthRemaining);

        public TextBoxViewBindingModel<QuestionnaireView, QuestionnaireViewModel> SurnameTextBoxViewBindingModel
            => GetStandardTextBoxViewModel(
                vw =>vw.Surname,
                vm => vm.Surname);

        public LabelViewBindingModel<QuestionnaireView, QuestionnaireViewModel> SurnameLengthRemainingLabelViewBindingModel
            => GetStandardLengthRemainingLabelViewBindingModel(
                vw => vw.SurnameLengthRemaining,
                vm => vm.SurnameLengthRemaining);

        public TextBoxViewBindingModel<QuestionnaireView, QuestionnaireViewModel> AnswerOneTextBoxViewBindingModel
            => GetStandardTextBoxViewModel(
                vw =>vw.AnswerOne,
                vm => vm.AnswerOne);

        public LabelViewBindingModel<QuestionnaireView, QuestionnaireViewModel> AnswerOneRemainingLabelViewBindingModel
            => GetStandardLengthRemainingLabelViewBindingModel(
                vw => vw.AnswerOneLengthRemaining,
                vm => vm.AnswerOneLengthRemaining);

        public TextBoxViewBindingModel<QuestionnaireView, QuestionnaireViewModel> AnswerTwoTextBoxViewBindingModel
            => GetStandardTextBoxViewModel(
                vw =>vw.AnswerTwo,
                vm => vm.AnswerTwo);

        public LabelViewBindingModel<QuestionnaireView, QuestionnaireViewModel> AnswerTwoRemainingLabelViewBindingModel
            => GetStandardLengthRemainingLabelViewBindingModel(
                vw => vw.AnswerTwoLengthRemaining,
                vm => vm.AnswerTwoLengthRemaining);

        public TextBoxViewBindingModel<QuestionnaireView, QuestionnaireViewModel> AnswerThreeTextBoxViewBindingModel
            => GetStandardTextBoxViewModel(
                vw =>vw.AnswerThree,
                vm => vm.AnswerThree);

        public LabelViewBindingModel<QuestionnaireView, QuestionnaireViewModel> AnswerThreeRemainingLabelViewBindingModel
            => GetStandardLengthRemainingLabelViewBindingModel(
                vw => vw.AnswerThreeLengthRemaining,
                vm => vm.AnswerThreeLengthRemaining);

        public TextBoxViewBindingModel<QuestionnaireView, QuestionnaireViewModel> AnswerFourTextBoxViewBindingModel
            => GetStandardTextBoxViewModel(
                vw =>vw.AnswerFour,
                vm => vm.AnswerFour);

        public LabelViewBindingModel<QuestionnaireView, QuestionnaireViewModel> AnswerFourRemainingLabelViewBindingModel
            => GetStandardLengthRemainingLabelViewBindingModel(
                vw => vw.AnswerFourLengthRemaining,
                vm => vm.AnswerFourLengthRemaining);

        public TextBoxViewBindingModel<QuestionnaireView, QuestionnaireViewModel> AnswerFiveTextBoxViewBindingModel
            => GetStandardTextBoxViewModel(
                vw =>vw.AnswerFive,
                vm => vm.AnswerFive);

        public LabelViewBindingModel<QuestionnaireView, QuestionnaireViewModel> AnswerFiveRemainingLabelViewBindingModel
            => GetStandardLengthRemainingLabelViewBindingModel(
                vw => vw.AnswerFiveLengthRemaining,
                vm => vm.AnswerFiveLengthRemaining);

        public IEnumerable<IViewBindingModel<QuestionnaireView, QuestionnaireViewModel>> GetBindings()
        {
            yield return ForenameTextBoxViewBindingModel;
            yield return ForenameLengthRemainingLabelViewBindingModel;

            yield return SurnameTextBoxViewBindingModel;
            yield return SurnameLengthRemainingLabelViewBindingModel;

            yield return AnswerOneTextBoxViewBindingModel;
            yield return AnswerOneRemainingLabelViewBindingModel;

            yield return AnswerTwoTextBoxViewBindingModel;
            yield return AnswerTwoRemainingLabelViewBindingModel;

            yield return AnswerThreeTextBoxViewBindingModel;
            yield return AnswerThreeRemainingLabelViewBindingModel;

            yield return AnswerFourTextBoxViewBindingModel;
            yield return AnswerFourRemainingLabelViewBindingModel;

            yield return AnswerFiveTextBoxViewBindingModel;
            yield return AnswerFiveRemainingLabelViewBindingModel;

        }

        private LabelViewBindingModel<QuestionnaireView, QuestionnaireViewModel> GetStandardLengthRemainingLabelViewBindingModel(
            Expression<Func<QuestionnaireView, Label>> controlExpression,
            Expression<Func<QuestionnaireViewModel, object>> viewModelNumberExpression)
        {
            return new(controlExpression)
            {
                // TODO: explore the ability to pass in an object and not apply a vm convertor
                Content = new OneWayBind<QuestionnaireViewModel, object>(viewModelNumberExpression, o => o.ToString()),
                // TODO: explore the ability to pass in a type (int) and convert to brush
                //Foreground = new OneWayBind<QuestionnaireViewModel, Brush>()
            };
        }

        private static TextBoxViewBindingModel<QuestionnaireView, QuestionnaireViewModel> GetStandardTextBoxViewModel(
            Expression<Func<QuestionnaireView, TextBox>> controlExpression,
            Expression<Func<QuestionnaireViewModel, string>> viewModelTextExpression)
        {
            return new(controlExpression)
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
