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
    public sealed class QuestionnaireViewBindingModels : AbstractEnableViewToViewModelBindings<QuestionnaireView, QuestionnaireViewModel>
    {
        protected override IEnumerable<IViewBindingModel<QuestionnaireView, QuestionnaireViewModel>> GetBindings()
        {
            // launch interaction
            yield return new ButtonViewBindingModel<QuestionnaireView, QuestionnaireViewModel>(vw => vw.LaunchInteraction)
            {
                Command = new CommandBinding<QuestionnaireViewModel>(vm => vm.LaunchInteraction)
            };

            // Forename
            yield return GetStandardTextBoxViewModel(
                vw => vw.Forename,
                vm => vm.Forename);
            yield return GetStandardLengthRemainingLabelViewBindingModel(
                vw => vw.ForenameLengthRemaining,
                vm => vm.ForenameLengthRemaining,
                vm => vm.ForenameLengthRemaining);

            // Surname
            yield return GetStandardTextBoxViewModel(
                vw =>vw.Surname,
                vm => vm.Surname);
            yield return GetStandardLengthRemainingLabelViewBindingModel(
                vw => vw.SurnameLengthRemaining,
                vm => vm.SurnameLengthRemaining,
                vm => vm.SurnameLengthRemaining);

            // answer one
            yield return GetStandardTextBoxViewModel(
                vw =>vw.AnswerOne,
                vm => vm.AnswerOne);
            yield return GetStandardLengthRemainingLabelViewBindingModel(
                vw => vw.AnswerOneLengthRemaining,
                vm => vm.AnswerOneLengthRemaining,
                vm => vm.AnswerOneLengthRemaining);

            // answer two
            yield return GetStandardTextBoxViewModel(
                vw =>vw.AnswerTwo,
                vm => vm.AnswerTwo);
            yield return GetStandardLengthRemainingLabelViewBindingModel(
                vw => vw.AnswerTwoLengthRemaining,
                vm => vm.AnswerTwoLengthRemaining,
                vm => vm.AnswerTwoLengthRemaining);

            // answer three
            yield return GetStandardTextBoxViewModel(
                vw =>vw.AnswerThree,
                vm => vm.AnswerThree);
            yield return GetStandardLengthRemainingLabelViewBindingModel(
                vw => vw.AnswerThreeLengthRemaining,
                vm => vm.AnswerThreeLengthRemaining,
                vm => vm.AnswerThreeLengthRemaining);

            // answer four
            yield return GetStandardTextBoxViewModel(
                vw =>vw.AnswerFour,
                vm => vm.AnswerFour);
            yield return GetStandardLengthRemainingLabelViewBindingModel(
                vw => vw.AnswerFourLengthRemaining,
                vm => vm.AnswerFourLengthRemaining,
                vm => vm.AnswerFourLengthRemaining);

            // answer five
            yield return GetStandardTextBoxViewModel(
                vw =>vw.AnswerFive,
                vm => vm.AnswerFive);
            yield return GetStandardLengthRemainingLabelViewBindingModel(
                vw => vw.AnswerFiveLengthRemaining,
                vm => vm.AnswerFiveLengthRemaining,
                vm => vm.AnswerFourLengthRemaining);
        }

        private LabelViewBindingModel<QuestionnaireView, QuestionnaireViewModel> GetStandardLengthRemainingLabelViewBindingModel(
            Expression<Func<QuestionnaireView, Label>> controlExpression,
            Expression<Func<QuestionnaireViewModel, object>> viewModelObjectExpression,
            Expression<Func<QuestionnaireViewModel, int>> viewModelNumberExpression)
        {
            return new(controlExpression)
            {
                // TODO: explore the ability to pass in an object and not apply a vm convertor. this is doing boxing we can probably avoid
                Content = new OneWayBind<QuestionnaireViewModel, object>(viewModelObjectExpression, o => o?.ToString()),
                Foreground = new OneWayBindingWithConversionOnOneOrTwoWay<QuestionnaireViewModel, Brush, int>(viewModelNumberExpression, lengthRemaining => GetBrushForLengthRemaining(lengthRemaining))
                //Foreground = new OneWayBind<QuestionnaireViewModel, Brush>()
            };
        }

        private static Brush GetBrushForLengthRemaining(int lengthRemaining)
        {
            return lengthRemaining switch
            {
                < 0 => Brushes.Red,
                < 10 => Brushes.OrangeRed,
                < 20 => Brushes.Orange,
                _ => Brushes.Black
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
